﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PostHost.Models
{
    public class ContentViewModels
    {
        public Content theContent { get; set; }
        public List<TaggerViewModel> theTags { get; set; }

    }

    public class LikeViewModels
    {
        public int theLikes { get; set; }
        public int theDislikes { get; set; }
    }
}