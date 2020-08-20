namespace MonitoreoDiscosService
{
    partial class ServicioMonitoreoDiscos
    {
        /// <summary> 
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar 
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.eventLog = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog)).BeginInit();
            // 
            // ServicioMonitoreoDiscos
            // 
            this.ServiceName = "Monitoreo Discos";
            //
            // timer
            //
            this.timer = new System.Timers.Timer();
            this.timer.Enabled = true;
            this.timer.Interval = 60000;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            ((System.ComponentModel.ISupportInitialize)(this.eventLog)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog eventLog;
        private System.Timers.Timer timer;
    }
}
