using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ValidateFtpApp.Models
{
    public class FtpServerDetails
    {
        [Required(ErrorMessage = "Host can't be empty or blank.!")]
        public String Host { get; set; }

        [Required(ErrorMessage = "Port can't be empty or blank.!")]
        public int? Port { get; set; }

        [Required(ErrorMessage = "User Name can't be empty or blank.!")]
        public String UserName { get; set; }

        [Required(ErrorMessage = "Please select the ppk file.!")]
        public IFormFile postedFile { set; get; }

        //[Required(ErrorMessage = "Ssh Host Key Fingerprint can't be empty or blank.!")]
        public String SshHostKeyFingerprint { get; set; }
        
        public String Password { get; set; }
    }
}
