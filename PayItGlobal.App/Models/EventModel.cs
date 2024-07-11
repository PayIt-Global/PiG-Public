﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayItGlobal.App.Models;

record EventModel(string Title, string Author, string AvatarImage, string ImageSource, string ImageSourceDetails, DateTime Date)
{
    public static EventModel[] Featured
        => new[]
        {
            new EventModel(
                Title: "Sit amet odio nisi leo viverra sed a vel blandit adipiscing",
                ImageSource: "event1.png",
                ImageSourceDetails: "event1_details.png",
                Author: "Samata Smith",
                AvatarImage: "avatar1.png",
                Date: DateTime.Now.AddMonths(-12)),
            new EventModel(
                Title: "In venenatis condimentum mattis nulla tincidunt. Porta quis risus.",
                ImageSource: "event2.png",
                ImageSourceDetails: "event2_details.png",
                Author: "Marvin McKinney",
                AvatarImage: "avatar2.png",
                Date: DateTime.Now.AddMonths(-12)),
            new EventModel(
                Title: "Sit amet odio nisi leo viverra sed a vel blandit adipiscing",
                ImageSource: "event1.png",
                ImageSourceDetails: "event1_details.png",
                Author: "Samata Smith",
                AvatarImage: "avatar1.png",
                Date: DateTime.Now.AddMonths(-12)),
            new EventModel(
                Title: "In venenatis condimentum mattis nulla tincidunt. Porta quis risus.",
                ImageSource: "event2.png",
                ImageSourceDetails: "event2_details.png",
                Author: "Marvin McKinney",
                AvatarImage: "avatar2.png",
                Date: DateTime.Now.AddMonths(-12)),
            new EventModel(
                Title: "Sit amet odio nisi leo viverra sed a vel blandit adipiscing",
                ImageSource: "event1.png",
                ImageSourceDetails: "event1_details.png",
                Author: "Samata Smith",
                AvatarImage: "avatar1.png",
                Date: DateTime.Now.AddMonths(-12)),
            new EventModel(
                Title: "In venenatis condimentum mattis nulla tincidunt. Porta quis risus.",
                ImageSource: "event2.png",
                ImageSourceDetails: "event2_details.png",
                Author: "Marvin McKinney",
                AvatarImage: "avatar2.png",
                Date: DateTime.Now.AddMonths(-12)),
        };
};
