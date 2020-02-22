using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lab2.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lab2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IGaseosaSettings _agua;
        public ValuesController(IGaseosaSettings moviesettings)
        {
            _agua = moviesettings;
        }
        //localhost:51626/api/Values/GetWithParam/?nombre=""
        [HttpGet("GetWithParam", Name = "Get")]
        public ActionResult<object> Get(string nombre)
        {
            object tempo = _agua.GetOne(nombre);
            return tempo;
        }

        [HttpPost]
        public ActionResult<Gaseosa> Create(Gaseosa gas)
        {            
            _agua.Create(gas);
            Disck (gas);
            return gas;
        }
        private void Disck(Gaseosa elementeo)
        {
            var jsonPatientList = JsonConvert.SerializeObject(elementeo);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "GaseosasData/");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var UploadedFile = Request.Form.Files[0];
            if (UploadedFile!=null)
            {
                string ServerDirectory = Directory.GetCurrentDirectory();
                string UpFilesDirectoryPath = Path.Combine(ServerDirectory, "GaseosasData/");
                string WServerPath = UpFilesDirectoryPath + Path.GetFileName(UploadedFile.FileName.Replace(".txt", "ZigZagEncoded.txt"));
            }
        }
    }
}
