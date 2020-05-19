using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace DayVsNight.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private int eventCount;
        private string userImage;
        private string username;
        private ObservableCollection<SecurityZone> zones;

        public string UserName 
        { 
            get => username;
            set => SetProperty(ref username, value);
        }
        public string UserImage 
        { 
            get => userImage;
            set => SetProperty(ref userImage, value);
        }

        public int EventCount 
        { 
            get => eventCount;
            set => SetProperty(ref eventCount, value);
        }

        public ObservableCollection<SecurityZone> Zones
        {
            get => zones;
            set => SetProperty(ref zones, value);
        }

        public MainViewModel()
        {
            UserName = "Kym";
            UserImage = "profile.png";
            EventCount = 2;

            Zones = new ObservableCollection<SecurityZone>()
            {
                new SecurityZone {Name="Zone1", Image="Room1"},
                new SecurityZone {Name="Zone2", Image="Room2"},
                new SecurityZone {Name="Zone3", Image="Room3"},
            };
        }

    }

    public class SecurityZone : BaseViewModel
    {
        private string name;
        private string image;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }
    }
}
