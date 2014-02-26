using System;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AndroidToolkit.Helpers.Admin
{
    internal sealed class AdminManager
    {
        public AdminManager()
        {
            _db = new AndroidToolkitDB();
        }
        public async Task<bool> LogIn(string username, string password)
        {
            try
            {
                return await LogInSuccess(username, password);
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("Error", ex.ToString(), 400, 200);
            }
            return false;

        }

        public string[] AdminData()
        {
            string[] data = new string[2];
            data[0] = string.Empty;
            data[1] = string.Empty;
            if (File.Exists("CurrentAdmin.xml"))
            {
                XDocument doc = XDocument.Load("CurrentAdmin.xml");
                XElement root = doc.Root;
                data[0] = (string)root.Element("Admin").Attribute("ID").Value;
                data[1] = (string)root.Element("Admin").Attribute("Username").Value;
            }
            return data;
        }

        public bool CreateAdmin(AndroidToolkitDB db, string name, string username, string password)
        {
            try
            {
                _db.Admins.Add(new AndroidToolkit.Admin() { Name = name, Username = username, Password = password });
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("Error", ex.ToString(), 400, 200);
                return false;
            }
        }
        public bool Edit(AndroidToolkitDB _db)
        {
            try
            {
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("Error", ex.ToString(), 400, 200);
                return false;
            }
        }
        public bool Delete(AndroidToolkitDB _db, AndroidToolkit.Admin _admin)
        {
            try
            {
                _db.Admins.Remove(_admin);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("Error", ex.ToString(), 400, 200);
                return false;
            }
        }
        public bool ChangePassword(AndroidToolkitDB db, int id, string newPassword, string oldPassword)
        {
            try
            {
                AndroidToolkit.Admin editAdmin = _db.Admins.FirstOrDefault(a => a.ID == id);
                if (editAdmin != null && editAdmin.Password == oldPassword)
                {
                    editAdmin.Password = newPassword;
                    _db.Admins.AddOrUpdate(editAdmin);
                    _db.SaveChanges();
                    return true;
                }
                //if (editAdmin != null)
                //{
                //    editAdmin.Password = newPassword;
                //    db.Admins.Attach(editAdmin);
                //    db.Entry(editAdmin).State = EntityState.Modified;
                //    db.SaveChanges();
                //    return true;
                //}
                return false;
            }
            catch (Exception ex)
            {
                MetroMessageBoxHelper.ShowBox("ERROR", ex.ToString(), 400, 200);
                return false;
            }


        }
        public string MasterAdminID
        {
            get
            {
                return _MasterAdminID;
            }
        }
        public string MasterAdminUsername
        {
            get
            {
                return _MasterAdminUsername;
            }
        }
        private const string _MasterAdminID = "1";
        private const string _MasterAdminUsername = "Gabrijel Boduljak - Administrator";


        private readonly AndroidToolkitDB _db;

        #region Tasks

        private async Task<bool> LogInSuccess(string username, string password)
        {
            foreach (var item in await _db.Admins.ToListAsync())
            {
                if ((item.Username == username) && (item.Password == password))
                {
                    XDocument doc = new XDocument();
                    XElement root = new XElement("AdminData");
                    XElement currentAdmin = new XElement("Admin");
                    XAttribute admin_id = new XAttribute("ID", item.ID.ToString());
                    XAttribute admin_name = new XAttribute("Name", item.Name);
                    XAttribute admin_username = new XAttribute("Username", item.Username);
                    currentAdmin.Add(admin_id, admin_name, admin_username);
                    root.Add(currentAdmin);
                    doc.Add(root);
                    await Task.Factory.StartNew(() => doc.Save("CurrentAdmin.xml"));
                    return true;

                }
                return false;
            }

            return false;
        }

        #endregion
    }
}
