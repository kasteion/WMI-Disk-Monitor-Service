using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MonitoreoDiscosService
{
    class ClMonitoreo:ClConexion
    {

        public TimeSpan SeleccionHoraPeriodicidad()
        {
            TimeSpan Hora = new TimeSpan();
            try 
            {
                Open();
                Comando.Parameters.Clear();
                Comando.CommandText = "Select Top 1 Hora from PERIODICIDAD Where Hora >= @TIEMPO Order by Hora";
                Comando.Parameters.Add("TIEMPO", SqlDbType.Time, 0).Value = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                Hora = (TimeSpan)Comando.ExecuteScalar();
                Close();
                return Hora;
            }
            catch
            {
                Close();
                return Hora;
            }
        }

        public DataTable SeleccionServidoresMonitoreo()
        {
            DataTable servidores = new DataTable();
            try
            { 
                Open();
                Comando.Parameters.Clear();
                Comando.CommandText = "Select Servidor, IP, ConsultarIP from SERVIDORES";
                Adaptador.Fill(servidores);
                Close();
                return servidores;
            }
            catch
            {
                Close();
                return servidores;
            }
        }

        public bool InsertMonitoreo(string Servidor, DateTime Fecha, string Drive, double Size, double Used, double Free, double Percentage)
        {
            try 
            {
                Open();
                Comando.Parameters.Clear();
                Comando.CommandText = "Insert into " + Servidor.Trim().ToUpper() + " Values (@FECHA, @DRIVE, @SIZE, @USED, @FREE, @PERCENTAGE)";
                Comando.Parameters.Add("FECHA", SqlDbType.DateTime).Value = Fecha;
                Comando.Parameters.Add("DRIVE", SqlDbType.NChar, 3).Value = Drive;
                Comando.Parameters.Add("SIZE", SqlDbType.Real).Value = Size;
                Comando.Parameters.Add("USED", SqlDbType.Real).Value = Used;
                Comando.Parameters.Add("FREE", SqlDbType.Real).Value = Free;
                Comando.Parameters.Add("PERCENTAGE", SqlDbType.Real).Value = Percentage;
                Comando.ExecuteNonQuery();
                Close();
                return true;
            }
            catch 
            {
                Close();
                return false;
            }
        }

        public DataTable SelectParametrosAlertas(string Servidor, string Drive)
        {
            DataTable Alertas = new DataTable();
            try 
            {
                Open();
                Comando.Parameters.Clear();
                Comando.CommandText = "Select Alerta, Emergencia from ALERTAS Where Servidor = @SERVIDOR and Drive = @DRIVE";
                Comando.Parameters.Add("SERVIDOR", SqlDbType.NChar, 50).Value = Servidor;
                Comando.Parameters.Add("DRIVE", SqlDbType.NChar, 3).Value = Drive;
                Adaptador.Fill(Alertas);
                Close();
                return Alertas;
            }
            catch
            {
                Close();
                return Alertas;
            }
        }

        public DataTable SelectDestinatarios(string Servidor)
        {
            DataTable Destinatarios = new DataTable();
            try 
            {
                Open();
                Comando.Parameters.Clear();
                Comando.CommandText = "Select Destinatario from DESTINATARIOS Union Select Destinatario from DESTINATARIOSERVIDOR Where Servidor = @SERVIDOR";
                Comando.Parameters.Add("SERVIDOR", SqlDbType.NChar, 50).Value = Servidor;
                Adaptador.Fill(Destinatarios);
                Close();
                return Destinatarios;
            }
            catch 
            {
                Close();
                return Destinatarios;
            }
        }

        public DataTable SelectRemitente()
        {
            DataTable Remitente = new DataTable();
            try 
            {
                Open();
                Comando.Parameters.Clear();
                Comando.CommandText = "Select Top 1 Remitente from REMITENTES";
                Adaptador.Fill(Remitente);
                Close();
                return Remitente;
            }
            catch
            {
                Close();
                return Remitente;
            }
        }

        public bool SelectTablaExiste(string NombreTabla)
        {
            DataTable Tabla = new DataTable();
            try 
            {
                Open();
                Comando.Parameters.Clear();
                Comando.CommandText = "Select * from INFORMATION_SCHEMA.Tables Where TABLE_CATALOG = 'Monitoreo' And TABLE_SCHEMA = 'dbo' and TABLE_NAME = '" + NombreTabla.Trim().ToUpper() + "' and TABLE_TYPE = 'BASE TABLE'";
                Adaptador.Fill(Tabla);
                Close();
                if (Tabla.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                Close();
                if (Tabla.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CreateTable(string Servidor)
        {
            try
            {
                Open();
                Comando.Parameters.Clear();
                Comando.CommandText = "CREATE TABLE [dbo].[" + Servidor.Trim().ToUpper() + "]([Fecha] [datetime] NOT NULL,[Drive] [nchar](3) NOT NULL,[Size] [real] NULL,[Used] [real] NULL,[Free] [real] NULL,[PorcentajeFree] [real] NULL,CONSTRAINT [PK_" + Servidor.Trim().ToUpper() + "] PRIMARY KEY CLUSTERED ([Fecha] ASC,[Drive] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]";
                Comando.ExecuteNonQuery();
                Close();
                return true;
            }
            catch
            {
                Close();
                return false;
            }
        }

        public bool InsertAlarma(string Servidor, string Drive, double Alerta, double Emergencia)
        {
            try
            {
                Open();
                Comando.Parameters.Clear();
                Comando.CommandText = "Insert Into ALERTAS Values (@SERVIDOR, @DRIVE, @ALERTA, @EMERGENCIA)";
                Comando.Parameters.Add("SERVIDOR", SqlDbType.NChar, 50).Value = Servidor.Trim();
                Comando.Parameters.Add("DRIVE", SqlDbType.NChar, 3).Value = Drive.Trim().ToUpper();
                Comando.Parameters.Add("ALERTA", SqlDbType.Real).Value = Alerta;
                Comando.Parameters.Add("EMERGENCIA", SqlDbType.Real).Value = Emergencia;
                Comando.ExecuteNonQuery();
                Close();
                return true;
            }
            catch
            {
                Close();
                return false;
            }
        }
        
    }
}
