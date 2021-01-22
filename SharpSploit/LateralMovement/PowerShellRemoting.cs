using System;
using System.Linq;
using System.Security;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;

namespace SharpSploit.LateralMovement
{
    /// <summary>
    /// PowerShellRemoting is a class for creating PowerShell runspaces on a remote computer
    /// and executing the specified command.
    /// </summary>
    public class PowerShellRemoting
    {
        /// <summary>
        /// Invoke a PowerShell command on a remote machine.
        /// </summary>
        /// <param name="ComputerName">ComputerName of remote system to execute process.</param>
        /// <param name="PowerShellCode">Command to execute on remote system.</param>
        /// <param name="OutString">Switch. If true, appends Out-String to the PowerShellCode to execute.</param>
        /// <param name="Domain">Domain for explicit credentials.</param>
        /// <param name="Username">Username for explicit credentials.</param>
        /// <param name="Password">Password for explicit credentials.</param>
        /// <returns>String. Returns the result of the PowerShell command.</returns>
        /// <author>Daniel Duggan (@_RastaMouse)</author>
        public static string InvokeCommand(string ComputerName, string PowerShellCode, bool OutString = true, string Domain = "", string Username = "", string Password = "")
        {
            string output;
            WSManConnectionInfo connectionInfo;
            bool useCredentials = Domain != "" && Username != "" && Password != "";

            Uri targetUri = new Uri($"http://{ComputerName}:5985/WSMAN");

            if (useCredentials)
            {
                SecureString securePassword = new SecureString();
                foreach (char c in Password.ToCharArray())
                {
                    securePassword.AppendChar(c);
                }

                PSCredential psCredential = new PSCredential($"{Domain}\\{Username}", securePassword);
                connectionInfo = new WSManConnectionInfo(targetUri, "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", psCredential);
            }
            else
            {
                connectionInfo = new WSManConnectionInfo(targetUri);
            }

            using (Runspace remoteRunspace = RunspaceFactory.CreateRunspace(connectionInfo))
            {
                try
                {
                    remoteRunspace.Open();

                    using (PowerShell ps = PowerShell.Create())
                    {
                        ps.Runspace = remoteRunspace;
                        ps.AddScript(PowerShellCode);

                        if (OutString) { ps.AddCommand("Out-String"); }

                        PSDataCollection<object> results = new PSDataCollection<object>();
                        ps.Streams.Error.DataAdded += (sender, e) =>
                        {
                            Console.WriteLine("Error");
                            foreach (ErrorRecord er in ps.Streams.Error.ReadAll())
                            {
                                results.Add(er);
                            }
                        };
                        ps.Streams.Verbose.DataAdded += (sender, e) =>
                        {
                            foreach (VerboseRecord vr in ps.Streams.Verbose.ReadAll())
                            {
                                results.Add(vr);
                            }
                        };
                        ps.Streams.Debug.DataAdded += (sender, e) =>
                        {
                            foreach (DebugRecord dr in ps.Streams.Debug.ReadAll())
                            {
                                results.Add(dr);
                            }
                        };
                        ps.Streams.Warning.DataAdded += (sender, e) =>
                        {
                            foreach (WarningRecord wr in ps.Streams.Warning)
                            {
                                results.Add(wr);
                            }
                        };

                        ps.Invoke(null, results);
                        output = string.Join(Environment.NewLine, results.Select(R => R.ToString()).ToArray());
                        ps.Commands.Clear();
                        return output;
                    }
                }
                catch (PSRemotingTransportException e)
                {
                    output = e.GetType().FullName + ": " + e.Message + Environment.NewLine + e.StackTrace;
                }

                remoteRunspace.Close();
            }

            return output;
        }
    }
}
