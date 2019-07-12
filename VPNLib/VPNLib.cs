using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotRas;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.IO;

namespace VPNLib
{



    public class VpnMonitor
    {
        RasPhoneBook rasPhoneBook1 = new RasPhoneBook();
        RasDialer RasDialer1 = new RasDialer();
        private RasHandle handle = null;
        RasEntry entry;
        RasDevice device = RasDevice.GetDeviceByName("(PPTP)", RasDeviceType.Vpn);
        string vpnuser;
        string ip_address;
        string username;
        string password;
        //string vpnConfigPatch = @"C:\SeleniumServer\VpnAccess.json";
        string BaseDirectory;
        string vpnConfigPatch;
        /// <summary>
        /// przykładowe połączenie dla Chester!
        /// </summary>       

        #region konstruktory
        public VpnMonitor(string vpnuser, string ip_address, string username, string password)
        {
            string appFileName = Environment.GetCommandLineArgs()[0];
            BaseDirectory = Path.GetDirectoryName(appFileName);
            this.vpnuser = vpnuser;
            this.ip_address = ip_address;
            this.username = username;
            this.password = password;
            entry = RasEntry.CreateVpnEntry(vpnuser, ip_address, RasVpnStrategy.PptpOnly, device, false);
        }
        public VpnMonitor()
        {
            try
            {

                string appFileName = Environment.GetCommandLineArgs()[0];
                BaseDirectory = Path.GetDirectoryName(appFileName);
                //  vpnConfigPatch = BaseDirectory + @"\SeleniumServer\VpnAccess.json";
                vpnConfigPatch = BaseDirectory + ConstantVariables.ChesterServerPath + ConstantVariables.VpnAccessJsonName;
                JObject VpnSettings = JsonDataSerializer.LoadJsonFromFile(vpnConfigPatch);
                vpnuser = (string)VpnSettings.SelectToken("vpnuser");
                ip_address = (string)VpnSettings.SelectToken("ip_address");
                username = (string)VpnSettings.SelectToken("username");
                password = (string)VpnSettings.SelectToken("password");
                entry = RasEntry.CreateVpnEntry(vpnuser, ip_address, RasVpnStrategy.PptpOnly, device, false);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion

        #region stwórz VPN
        /// <summary>
        /// Stwórz połączenie VPN true w przypatku powodzenia.
        /// </summary>
        /// nazwa połączenia vpn
        /// <param name="vpnuser"></param>
        /// adres servera vpn
        /// <param name="ip_address"></param>
        /// <returns></returns>
        public bool VPNCreate(string vpnuser, string ip_address)
        {
            this.rasPhoneBook1.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers));
            // RasEntry entry = RasEntry.CreateVpnEntry(vpnuser, ip_address, RasVpnStrategy.Default, RasDevice.GetDeviceByName("(PPTP)", RasDeviceType.Vpn, false));
            RasDialer1.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            try
            {
                this.rasPhoneBook1.Entries.Add(entry);
                return true;
            }
            catch
            { return false; }
        }
        public bool VPNCreate()
        {
            this.rasPhoneBook1.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers));
            RasDialer1.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            try
            {
                this.rasPhoneBook1.Entries.Add(entry);
                return true;
            }
            catch
            { return false; }
        }
        #endregion

        #region Połącz VPN
        /// <summary>
        /// Nawiąż połączenie vpn
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// nazwa połączenia
        /// <param name="vpnuser"></param>
        /// <returns></returns>
        public bool VPNConnect(string username, string password, string vpnuser)
        {

            RasDialer1.EntryName = vpnuser;
            RasDialer1.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            RasDialer1.Credentials = new NetworkCredential(username, password);
            try
            {
                this.handle = this.RasDialer1.Dial();
                handle.Close();
                return true;

            }
            catch
            {
                return false;
            }

        }

        public bool VPNConnect()
        {

            RasDialer1.EntryName = vpnuser;
            RasDialer1.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            RasDialer1.Credentials = new NetworkCredential(username, password);
            try
            {
                this.handle = this.RasDialer1.Dial();
                handle.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

        #region Rozłącz połączenie
        /// <summary>
        /// rozłącz połączenie VPN
        /// </summary>
        /// <returns></returns>
        public bool VPNDisconnect()
        {
            try
            {
                if (RasDialer1.IsBusy)
                {
                    RasDialer1.DialAsyncCancel();
                }
                else
                {
                    RasConnection connection = RasConnection.GetActiveConnections().Where(o => o.EntryName == vpnuser).FirstOrDefault();
                    if (connection != null)
                    {
                        connection.HangUp();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        public bool DeleteVPN()
        {

            try
            {
                VPNDisconnect();
            }
            catch
            {

            }
            try
            {
                rasPhoneBook1.Entries.Remove(entry);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region sprawdź połączenie
        /// <summary>
        /// Sprawdź połączenie i zwróć jeden z 4 stanów
        /// </summary>
        /// <returns></returns>
        VpnStatus VPNConnectCheck()
        {

            try
            {
                rasPhoneBook1.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers));
                bool result = rasPhoneBook1.Entries.Contains(entry.Name);
                if (!result)
                {
                    return VpnStatus.doesntexist;
                }
                DotRas.RasConnection connection = RasConnection.GetActiveConnections().Where(o => o.EntryName == vpnuser).FirstOrDefault();
                if (connection != null && connection.EntryName == vpnuser)
                {
                    return VpnStatus.connected;
                }
                else
                {
                    return VpnStatus.disconnected;
                }
            }
            catch
            { return VpnStatus.error; }
            //try
            //{
            //    RasConnection connection = RasConnection.GetActiveConnectionByHandle(handle);
            //    return VpnStatus.connected;
            //}
            //catch
            //{ return VpnStatus.disconnected; }

            //return VpnStatus.doesntexist;
        }
        #endregion

        #region testuj połączenie
        /// <summary>
        /// sprawdza a w przypadku wystąpienia problemu próbuje przywrócić połączenie vpn
        /// </summary>
        /// <returns></returns>
        public bool AssureConnection()
        {
            try
            {
                VpnMonitor.VpnStatus status = new VpnMonitor.VpnStatus();
                status = VPNConnectCheck();
                switch (status)
                {
                    case VpnMonitor.VpnStatus.connected:
                        return true;

                    case VpnMonitor.VpnStatus.disconnected:
                        if (VPNConnect())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case VpnMonitor.VpnStatus.doesntexist:
                        if (VPNCreate())
                        {
                            Thread.Sleep(10000);
                            if (VPNConnect())
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                        else
                        {
                            return false;
                        }
                    case VpnMonitor.VpnStatus.error:
                        return false;
                    default:
                        return false;

                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        // void setIP(RasConnection connection)
        //{

        //    vpnIP = connection.GetProjectionInfo(RasProjectionType.IP).ToString();
        //}

        public string IP
        {
            get
            {
                try
                {
                    DotRas.RasConnection connection = RasConnection.GetActiveConnections().Where(o => o.EntryName == vpnuser).FirstOrDefault();
                    if (connection != null)
                    {
                        RasIPInfo ip = ((RasIPInfo)connection.GetProjectionInfo(RasProjectionType.IP));
                        return ip.IPAddress.ToString();
                    }
                    else
                    {
                        if (AssureConnection())
                        {
                            return IP;

                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
            }
        }
        #endregion

        #region Statusy Vpn
        enum VpnStatus : int { connected = 1, disconnected = 2, doesntexist = 3, error = 4 };
        #endregion

        #region ładowanie danych dostępowych z pliku

        void LoadVPN()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw (ex);
            }


        }

        #endregion
    }
}
