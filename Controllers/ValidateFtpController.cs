using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ValidateFtpApp.Models;
using System.IO;
using WinSCP;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace ValidateFtpApp.Controllers
{
    public class ValidateFtpController : Controller
    {
        private readonly FtpPPkeyPath _ftpPPkeyPath;
        public ValidateFtpController(IOptions<FtpPPkeyPath> ftpPPkeyPath)
        {
            _ftpPPkeyPath = ftpPPkeyPath.Value;
        }
        public IActionResult Index()
        {
            return View(new FtpServerDetails() { Host = String.Empty, Password = string.Empty, Port = null, UserName = String.Empty });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(FtpServerDetails item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (item.postedFile == null)
                    {
                        var errorMSG = "Please select the ppk file.!";
                        TempData["errorMSG"] = errorMSG;
                        return View(item);
                    }
                    else
                    {
                        string fileName = Path.GetFileName(item.postedFile.FileName);
                        String pathD = Path.Combine(_ftpPPkeyPath.FtpPkeyPath, fileName);
                        if (!System.IO.File.Exists(pathD))
                        {
                            System.IO.File.Delete(pathD);
                        }
                        else
                        {
                            using (FileStream stream = new FileStream(pathD, FileMode.Create))
                            {
                                item.postedFile.CopyTo(stream);
                            }
                        }
                        SessionOptions sessionOptions = new SessionOptions()
                        {
                            Protocol = Protocol.Sftp,
                            HostName = item.Host,
                            UserName = item.UserName,
                            PortNumber = item.Port ?? 0,
                            //SshHostKeyFingerprint = "ssh-rsa 2048 06kDJPc7t6mkAF9fGSAEoIhDIKogzYNjNzDI39+ukYk",

                            //SshHostKeyFingerprint = "ssh-rsa 3072 ncRvu/xO7719msYib/JSnW0BiWEWIfzz2HZyiC5NMQY",

                            //SshHostKeyFingerprint = "ssh-rsa 3072 +kRS6B7EtkmNHmaKUJ5sYk8HVf5SdJT1IA6UvY3X+S8",
                            SshHostKeyFingerprint = item.SshHostKeyFingerprint == null ? "ssh-rsa 2048 06kDJPc7t6mkAF9fGSAEoIhDIKogzYNjNzDI39+ukYk" : item.SshHostKeyFingerprint,
                            SshPrivateKeyPath = pathD,
                        };
                        using (WinSCP.Session session = new WinSCP.Session())
                        {
                            session.Open(sessionOptions);

                            //TransferOptions transferOptions = new TransferOptions();
                            //transferOptions.TransferMode = TransferMode.Binary;

                            //TransferOperationResult transferResult;
                            //transferResult = session.PutFiles(@"d:\toupload\*", "/home/user/", false, transferOptions);
                            //// Throw on any error
                            //transferResult.Check();
                            //// Print results
                            //foreach (TransferEventArgs transfer in transferResult.Transfers)
                            //{
                            //    Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                            //}
                            // Your code
                            TempData["errorMSG"] = "SFTP Server sucessfully connected Or key is valid.!";
                        }
                        return View(item);
                    }
                }
                catch (Exception ex)
                {
                    var errorMSG = ex.Message;
                    TempData["errorMSG"] = errorMSG;
                    return View(item);
                }
            }
            else
            {
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult GetSshHostKeyFingerprint(IFormFile file,String Host,int? Port,String UserName,String sshHostKeyFingerprint)
        {
            var SshHostKeyFingerprint = String.Empty;
            try
            {
                if (file != null)
                { 
                    string fileName = Path.GetFileName(file.FileName);
                    String pathD = Path.Combine(_ftpPPkeyPath.FtpPkeyPath, fileName);
                    if (!System.IO.File.Exists(pathD))
                    {
                        System.IO.File.Delete(pathD);
                    }
                    else
                    {
                        using (FileStream stream = new FileStream(pathD, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                    }
                    SessionOptions sessionOptions = new SessionOptions()
                    {
                        Protocol = Protocol.Sftp,
                        HostName = Host,
                        UserName = UserName,
                        PortNumber = Port ?? 0,
                        SshHostKeyFingerprint = sshHostKeyFingerprint == null ? "ssh-rsa 2048 06kDJPc7t6mkAF9fGSAEoIhDIKogzYNjNzDI39+ukYk" : sshHostKeyFingerprint,
                        SshPrivateKeyPath = pathD,
                    };
                    using (WinSCP.Session session = new WinSCP.Session())
                    {
                        session.Open(sessionOptions);
                        SshHostKeyFingerprint = sshHostKeyFingerprint;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (msg.Contains("Host key fingerprint is"))
                {
                    SshHostKeyFingerprint = Regex.Split(msg.Split(Regex.Match(msg, "Host key fingerprint is ").Value)[1], ".\r\n")[0]; //.Split('\r')[0].Split('"')[1];
                }
                else
                {
                    SshHostKeyFingerprint = String.Empty;
                }
            }
            var data = new
            {
                SshHostKeyFingerprint = SshHostKeyFingerprint
            };
            return Json(data);
        }

        //public IActionResult ClentIndex()
        //{
        //    return View(new FtpServerDetails() { Host = String.Empty, Password = string.Empty, Port = null, UserName = String.Empty });
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult ClentIndex(FtpServerDetails item)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            using (SftpClient sftpClient = new SftpClient(getSftpConnection(item.Host, item.UserName, item.Port ?? 0, _ftpPPkeyPath.DXCClientFtpPPkeyPath)))
        //            {
        //                sftpClient.Connect();
        //                TempData["errorMSG"] = "Sucessfully Connected.!";
        //                //using (FileStream fs = new FileStream("filePath", FileMode.Open))
        //                //{
        //                //    sftpClient.BufferSize = 1024;
        //                //    sftpClient.UploadFile(fs, Path.GetFileName("filePath"));
        //                //}
        //                sftpClient.Dispose();
        //            }
        //            return View(item);
        //        }
        //        catch (Exception ex)
        //        {
        //            var errorMSG = ex.Message;
        //            TempData["errorMSG"] = errorMSG;
        //            return View(item);
        //        }
        //    }
        //    else
        //    {
        //        return View(item);
        //    }
        //}

        //private static AuthenticationMethod[] privateKeyObject(string username, string publicKeyPath)
        //{
        //    PrivateKeyAuthenticationMethod privateKeyAuthenticationMethod;
        //    //using (var stream = new FileStream(publicKeyPath, System.IO.FileMode.Open, FileAccess.Read))
        //    //{
        //        var file = new PrivateKeyFile(@"C:\Users\karmi\OneDrive\Desktop\New folder\dxc352023-priv-key.ppk");
        //        privateKeyAuthenticationMethod = new PrivateKeyAuthenticationMethod(username, file);
        //        //var authMethod = new PrivateKeyAuthenticationMethod(ConfigurationHelper.User, file);

        //       // connection = new ConnectionInfo(ConfigurationHelper.HostName, ConfigurationHelper.Port, ConfigurationHelper.User, authMethod);
        //    //}
        //    //using (Stream stream = System.IO.File.OpenRead(publicKeyPath))
        //    //{
        //    //    PrivateKeyFile privateKeyFile = new PrivateKeyFile(stream);
        //    //    privateKeyAuthenticationMethod = new PrivateKeyAuthenticationMethod(username, privateKeyFile);
        //    //}
        //    return new AuthenticationMethod[] { privateKeyAuthenticationMethod };
        //}
        //public static ConnectionInfo getSftpConnection(string host, string username, int port, string publicKeyPath)
        //{
        //    return new ConnectionInfo(host, port, username, privateKeyObject(username, publicKeyPath));
        //}
    }
}
