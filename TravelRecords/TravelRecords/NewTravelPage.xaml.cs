using Plugin.Geolocator;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecords.Logic;
using TravelRecords.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelRecords
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewTravelPage : ContentPage
	{
		public NewTravelPage ()
		{
			InitializeComponent ();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync();

            var venues = await VenueLogic.GetVenues(position.Latitude, position.Longitude);
            venueListView.ItemsSource = venues;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                var selectedVenue = venueListView.SelectedItem as Venue;
                var firstCategory = selectedVenue.categories.FirstOrDefault();
                Post post = new Post()
                {
                    Experience = experienceEntry.Text,
                    CategoryId = firstCategory.id,
                    CategoryName = firstCategory.name,
                    Address = selectedVenue.location.address,
                    Distance = selectedVenue.location.distance,
                    Latitude = selectedVenue.location.lat,
                    Longitude = selectedVenue.location.lng,
                    VenueName = selectedVenue.name

                };

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Post>();
                    int rows = conn.Insert(post);

                    // at least one element was inserted into the database
                    if (rows > 0)
                        DisplayAlert("Success", "Experience successfully saved", "Ok");
                    else
                        DisplayAlert("Failure", "Experience failed to be saved", "Ok");
                }
            }
            catch(NullReferenceException nre) { DisplayAlert("Failure :nre", "Experience failed to be saved", "Ok"); }
            catch(Exception ex) { DisplayAlert("Failure :ex", "Experience failed to be saved ", "Ok"); }
        }
    }
}