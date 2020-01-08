using ConsultaMD.Areas.Identity.Pages.Account;
using ConsultaMD.Data;
using ConsultaMD.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsultaMD.Services
{
    public class MediumService : IMedium
    {
        private readonly ApplicationDbContext _context;
        public MediumService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Add(DoctorLocationsInputModel medium)
        {
            if(medium != null)
            {
                //CHECK INSURANCE
                var insuranceLocation = await _context.InsuranceLocations
                    .Include(i => i.MediumDoctor)
                    .FirstOrDefaultAsync(i => i.Id == medium.Selector).ConfigureAwait(false);
                if (insuranceLocation != null)
                {
                    //GET PLACE
                    var place = await GetPlace(medium.PlaceId).ConfigureAwait(false);
                    //CROSS CHECK LOCATION
                    if(place.CommuneId != insuranceLocation.CommuneId)
                    {
                        return;
                    }
                    //UPDATE||ADD PLACE
                    var dbplace = await _context.Places.SingleOrDefaultAsync(p => p.Id == place.Id).ConfigureAwait(false);
                    if (dbplace == null)
                        await _context.Places.AddAsync(place).ConfigureAwait(false);
                    else
                    {
                        dbplace.Address = place.Address;
                        dbplace.CId = place.CId;
                        dbplace.CommuneId = place.CommuneId;
                        dbplace.Latitude = place.Latitude;
                        dbplace.Longitude = place.Longitude;
                        dbplace.Name = place.Name;
                        dbplace.PhotoId = place.PhotoId;
                        _context.Places.Update(dbplace);
                    }
                    await _context.SaveChangesAsync().ConfigureAwait(false);

                    //CHECK OFFICE
                    var medicalOffice = await _context.MedicalOffices
                        .FirstOrDefaultAsync(o =>
                        o.PlaceId == medium.PlaceId
                        && o.Block == medium.Block
                        && o.Floor == medium.Floor
                        && o.Appartment == medium.Appartment
                        && o.Office == medium.Office).ConfigureAwait(false);

                    if (medicalOffice == null)
                    {
                        medicalOffice = new MedicalOffice
                        {
                            PlaceId = medium.PlaceId,
                            Block = medium.Block,
                            Floor = medium.Floor,
                            Appartment = medium.Appartment,
                            Office = medium.Office,
                        };
                        await _context.MedicalOffices.AddAsync(medicalOffice).ConfigureAwait(false);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                    insuranceLocation.MediumDoctor.MedicalAttentionMediumId = medicalOffice.Id;
                    insuranceLocation.MediumDoctor.PriceParticular = medium.Price;
                    insuranceLocation.MediumDoctor.OverTime = medium.HasOverTime;
                    _context.MediumDoctors.Update(insuranceLocation.MediumDoctor);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
        public async Task<Place> GetPlace(string id)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={id}&fields=name,url,geometry,formatted_address,photo,address_components&key=AIzaSyDkCLRdkB6VyOXs-Uz_MFJ8Ym9Ji1Xp3rA";
            var uri = new Uri(url);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(uri).ConfigureAwait(false);
                JObject json = JObject.Parse(response);
                JToken result = json["result"];
                JToken location = result["geometry"]["location"];
                var addr = result["address_components"];
                var comm = string.Empty;
                foreach(var comp in addr)
                {
                    if (comp["types"].ToString().Contains("locality", StringComparison.InvariantCultureIgnoreCase))
                    {
                        comm = (string)comp["long_name"];
                    }
                }
                var commune = await _context.Communes.SingleOrDefaultAsync(c => c.Name == comm).ConfigureAwait(false);
                return new Place 
                {
                    Id = id,
                    Latitude = double.Parse((string)location["lat"], CultureInfo.InvariantCulture),
                    Longitude = double.Parse((string)location["lng"], CultureInfo.InvariantCulture),
                    Name = (string)result["name"],
                    PhotoId = (string)result["photos"][0]["photo_reference"],
                    CId = ((string)result["url"]).Split("?")[1],
                    CommuneId = commune.Id,
                    Address = (string)result["formatted_address"]
                };
            }
        }
        public async Task Delete(int id) 
        {
            var medium = await _context.MediumDoctors
                .Include(m => m.MedicalAttentionMedium)
                .SingleOrDefaultAsync(m => m.Id == id)
                .ConfigureAwait(false);

            var officeId = medium.MedicalAttentionMediumId;
            medium.MedicalAttentionMediumId = null;

            _context.MediumDoctors.Update(medium);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            if(!await _context.MediumDoctors.AnyAsync(m => m.MedicalAttentionMediumId == officeId).ConfigureAwait(false))
            {
                var office = await _context.MedicalAttentionMediums.SingleOrDefaultAsync(m => m.Id == officeId).ConfigureAwait(false);
                _context.MedicalAttentionMediums.Remove(office);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        public async Task Enable(int id)
        {

        }
        public async Task Disable(int id)
        {

        }
    }
}
