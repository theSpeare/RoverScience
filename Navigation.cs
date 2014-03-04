using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{
    public class Navigation
    {
        public Rover rover
        {
            get
            {
                return RoverScience.Instance.rover;
            }
        }

        public double getDistanceBetweenTwoPoints(COORDS _from, COORDS _to)
        {

            double bodyRadius = FlightGlobals.ActiveVessel.mainBody.Radius;
            double dLat = (_to.latitude - _from.latitude).ToRadians();
            double dLon = (_to.longitude - _from.longitude).ToRadians();
            double lat1 = _from.latitude.ToRadians();
            double lat2 = _to.latitude.ToRadians();

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = bodyRadius * c;

            return Math.Round(d, 4);
        }


        public double getBearingFromCoords(COORDS target)
        {
            // Rover x,y position

            double dLat = (target.latitude - rover.location.latitude).ToRadians();
            double dLon = (target.longitude - rover.location.longitude).ToRadians();
            double lat1 = rover.location.latitude.ToRadians();
            double lat2 = target.latitude.ToRadians();

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) -
                Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

            double bearing = Math.Atan2(y, x).ToDegrees();
            //bearing = (bearing + 180) % 360;

            //return bearing % 360;
            return (bearing + 360) % 360;
        }
    }
}
