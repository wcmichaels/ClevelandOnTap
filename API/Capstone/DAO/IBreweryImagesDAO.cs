﻿using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.DAO
{
    public interface IBreweryImagesDAO
    {
        BreweryImage CreateImage(BreweryImage brewery);
        List<BreweryImage> GetImages(int breweryId);
    }
}
