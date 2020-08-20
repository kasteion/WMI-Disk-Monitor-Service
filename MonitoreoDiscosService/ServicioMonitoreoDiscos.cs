using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Management;
using System.Net.NetworkInformation;

namespace MonitoreoDiscosService
{
    public partial class ServicioMonitoreoDiscos : ServiceBase
    {
        public ServicioMonitoreoDiscos()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("MonitorDiscos"))
            {
                System.Diagnostics.EventLog.CreateEventSource("MonitorDiscos", "MonitorDiscosLog");
            }
            eventLog.Source = "MonitorDiscos";
            eventLog.Log = "MonitorDiscosLog";
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("Iniciando Servicio Monitoreo Discos");
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("Deteniendo Servicio Monitoreo Discos");
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //eventLog.WriteEntry("60 segundos...");
            try 
            {
                ClEnvioMail mail = new ClEnvioMail();
                ClMonitoreo monitoreo = new ClMonitoreo();
                TimeSpan Immalive = new TimeSpan(4, 0, 0);
                TimeSpan Ahora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                TimeSpan HoraProgramada = monitoreo.SeleccionHoraPeriodicidad();
                if (Ahora == HoraProgramada)
                {
                    string Serv = "";
                    string Serv1 = "";
                    DataTable Servidores = monitoreo.SeleccionServidoresMonitoreo();
                    for (int i = 0; i < Servidores.Rows.Count; i++)
                    {
                        if (Servidores.Rows[i]["ConsultarIP"].ToString().Trim().Equals("X"))
                        {
                            Serv = Servidores.Rows[i]["IP"].ToString().Trim();
                            Serv1 = Servidores.Rows[i]["Servidor"].ToString().Trim();
                            //Serv = Serv + " " + Servidores.Rows[i]["IP"].ToString().Trim();
                        }
                        else
                        {
                            Serv = Servidores.Rows[i]["Servidor"].ToString().Trim();
                            Serv1 = Servidores.Rows[i]["Servidor"].ToString().Trim();
                            //Serv = Serv + " " + Servidores.Rows[i]["Servidor"].ToString().Trim();
                        }
                        ManagementScope wmiScope = new ManagementScope(@"\\" + Serv + @"\root\cimv2");
                        ManagementObjectSearcher wmiSearcher = new ManagementObjectSearcher(wmiScope, new ObjectQuery(@"Select * from Win32_LogicalDisk WHERE DriveType=3"));
                        string Body = "";
                        try
                        {
                            foreach (ManagementObject wmiObject in wmiSearcher.Get())
                            {
                                try
                                {
                                    string nombre = wmiObject["Name"].ToString().Substring(0, 1);
                                    double porcentajeFreeSpace = Math.Round((double)((double.Parse(wmiObject["FreeSpace"].ToString()) / double.Parse(wmiObject["Size"].ToString())) * 100), 2);
                                    double size = Math.Round(double.Parse(wmiObject["Size"].ToString()) / 1073741824, 2);
                                    double freeSpace = Math.Round(double.Parse(wmiObject["FreeSpace"].ToString()) / 1073741824, 2);
                                    double usedSpace = Math.Round((double.Parse(wmiObject["Size"].ToString()) - long.Parse(wmiObject["FreeSpace"].ToString())) / 1073741824, 2);
                                    //eventLog.WriteEntry(Serv + " " + nombre + " " + porcentajeFreeSpace.ToString() + " " + size.ToString() + " " + freeSpace.ToString() + " " + usedSpace.ToString());
                                    //reviso si existe la tabla del servidor
                                    bool Existe = monitoreo.SelectTablaExiste(Serv1);
                                    if (!Existe)
                                    {
                                        //Si no existe la creo
                                        Existe = monitoreo.CreateTable(Serv1);
                                    }
                                    if (Existe)
                                    {
                                        monitoreo.InsertMonitoreo(Serv1, DateTime.Now, nombre, size, usedSpace, freeSpace, porcentajeFreeSpace);
                                        DataTable Alarmas = monitoreo.SelectParametrosAlertas(Serv1, nombre);
                                        //eventLog.WriteEntry("Alarmas: " + Alarmas.Rows.Count.ToString());
                                        if (Alarmas.Rows.Count > 0)
                                        {
                                            //eventLog.WriteEntry("FreeSpace: " + freeSpace.ToString() + " Emergencia: " + Alarmas.Rows[0]["Emergencia"].ToString());
                                            if (!(freeSpace > double.Parse(Alarmas.Rows[0]["Emergencia"].ToString())))
                                            {
                                                //eventLog.WriteEntry("Emergencia");
                                                Body = Body + "EMERGENCIA: \n";
                                                Body = Body + "     Disco: " + nombre + " \n";
                                                Body = Body + "     Espacio: " + size + "Gb \n";
                                                Body = Body + "     Utilizado: " + usedSpace + "Gb \n";
                                                Body = Body + "     Libre: " + freeSpace + "Gb \n";
                                                Body = Body + "     %Libre: " + porcentajeFreeSpace + "% \n\n";
                                            }
                                            else
                                            {
                                                //eventLog.WriteEntry("FreeSpace: " + freeSpace.ToString() + " Alerta: " + Alarmas.Rows[0]["Alerta"].ToString());
                                                if (!(freeSpace > double.Parse(Alarmas.Rows[0]["Alerta"].ToString())))
                                                {
                                                    //eventLog.WriteEntry("Alerta");
                                                    Body = Body + "ALERTA: \n";
                                                    Body = Body + "     Disco: " + nombre + " \n";
                                                    Body = Body + "     Espacio: " + size + "Gb \n";
                                                    Body = Body + "     Utilizado: " + usedSpace + "Gb \n";
                                                    Body = Body + "     Libre: " + freeSpace + "Gb \n";
                                                    Body = Body + "     %Libre: " + porcentajeFreeSpace + "% \n\n";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Si no tiene alarmas inserto una
                                            monitoreo.InsertAlarma(Serv1, nombre, freeSpace - 1, freeSpace - 2);
                                        }
                                    }
                                    else
                                    {
                                        eventLog.WriteEntry("Error al crear tabla para datos de: " + Serv1);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    eventLog.WriteEntry("Error monitoreo: " + Serv + "---" + ex.Message);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("El servidor RPC no está disponible."))
                            {
                                try
                                {
                                    Ping ping = new Ping();
                                    PingReply pingR = ping.Send(Serv);
                                    if (pingR.Status == IPStatus.Success)
                                    {
                                        Body = Body + "ALERTA: \n";
                                        Body = Body + "     El servidor no está siendo monitoreado:\n";
                                        Body = Body + "     " + ex.Message;
                                    }
                                    else
                                    {
                                        Body = Body + "ALERTA: \n";
                                        Body = Body + "     El servidor no está disponible.\n\n";
                                    }
                                }
                                catch
                                {
                                    Body = Body + "ALERTA: \n";
                                    Body = Body + "     El servidor no está disponible.\n\n";
                                }
                            }
                            else
                            {
                                Body = Body + "ALERTA: \n";
                                Body = Body + "     El servidor no está siendo monitoreado:\n";
                                Body = Body + "     " + ex.Message;
                            }
                        }
                        if (Body.Trim().Length > 0)
                        {
                            DataTable remitente = monitoreo.SelectRemitente();
                            DataTable destinatarios = monitoreo.SelectDestinatarios(Serv1);
                            if (remitente.Rows.Count > 0)
                            {
                                if (destinatarios.Rows.Count > 0)
                                {
                                    //eventLog.WriteEntry("Intenta enviar correo...");
                                    if (!(mail.EnvioMail(remitente.Rows[0][0].ToString().Trim(), destinatarios, "ALERTA DISCOS: " + Serv1, Body)))
                                    {
                                        eventLog.WriteEntry("Error al enviar correo por monitoreo de discos.");
                                    }
                                }
                            }
                        }
                    }
                    //eventLog.WriteEntry(Ahora.ToString() + " = " + HoraProgramada.ToString());
                }
                else
                {
                    if (Ahora == Immalive)
                    {
                        DataTable remitente = monitoreo.SelectRemitente();
                        DataTable destinatarios = monitoreo.SelectDestinatarios("");
                        if (remitente.Rows.Count > 0)
                        {
                            if (destinatarios.Rows.Count > 0)
                            {
                                //eventLog.WriteEntry("Intenta enviar correo...");
                                if (!(mail.EnvioMail(remitente.Rows[0][0].ToString().Trim(), destinatarios, "Up & Running", "El servicio de monitoreo de discos se esta ejecutando.")))
                                {
                                    eventLog.WriteEntry("Error al enviar correo por monitoreo de discos.");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry("Error monitoreo: " + ex.Message);
            }
        }
    }
}
