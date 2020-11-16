﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FacesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacesController : ControllerBase
    {
        [HttpPost]
        public async Task<List<byte[]>> ReadFaces()
        {
            using(var ms = new MemoryStream(2048))
            {
                await Request.Body.CopyToAsync(ms);
                var faces = Getfaces(ms.ToArray());
                return faces;
            }
        }

        private List<byte[]> Getfaces(byte[] image)
        {
            Mat src = Cv2.ImDecode(image, ImreadModes.Color);

            src.SaveImage("image.jpg", new ImageEncodingParam(ImwriteFlags.JpegProgressive, 255));
            var file = Path.Combine(Directory.GetCurrentDirectory(), "CascadeFile", "haarcascade_frontalface_default.xml");
            var faceCascade = new CascadeClassifier();
            faceCascade.Load(file);

            var faces = faceCascade.DetectMultiScale(src, 1.1, 6, HaarDetectionType.DoRoughSearch, new Size(60, 60));
            var facesList = new List<byte[]>();
            int j = 0;
            foreach (var rect in faces)
            {
                var faceImage = new Mat(src, rect);
                facesList.Add(faceImage.ToBytes(".jpg"));
                faceImage.SaveImage("face" + j + ".jpg", new ImageEncodingParam(ImwriteFlags.JpegProgressive, 255));
                j++;
            }
            return facesList;
        }
    }
}
