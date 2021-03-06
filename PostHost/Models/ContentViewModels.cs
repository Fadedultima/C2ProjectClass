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
        public List<Comment> theComments { get; set; }
        public LikeViewModels LVM { get; set; }
        public int uploadType { get; set; }
    }

    public class LikeViewModels
    {
        public bool hasLiked { get; set; }
        public bool hasDisliked { get; set; }
        public long contentId { get; set; }
        public int theLikes { get; set; }
        public int theDislikes { get; set; }
    }
}