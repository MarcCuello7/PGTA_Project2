using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Project2_Code
{
    class AsterixSimulation
    {
        public int simSpeed;
        public int recordIndex;
        public double time;
        public List<CAT48> CAT48list;
        public Dictionary<ushort, Aircraft> aircrafts;
        
        public AsterixSimulation(AsterixParser parser)
        {
            this.time = 8 * 3600;
            this.simSpeed = 1;
            this.recordIndex = 0;
            this.aircrafts = new Dictionary<ushort, Aircraft>();
            this.CAT48list = parser.CAT48list;
            this.CAT48list = this.CAT48list.OrderBy(o => o.TIME).ToList();
        }

        public void Update()
        {
            this.time += this.simSpeed;
            while ((this.recordIndex < this.CAT48list.Count) && 
                   (this.CAT48list[recordIndex].TIME <= this.time))
            {                
                CAT48 record = this.CAT48list[recordIndex];                
                ushort tracknumber = record.TN;
                if (!aircrafts.ContainsKey(tracknumber)) 
                {
                    aircrafts[tracknumber] = new Aircraft(record);
                }
                else
                {
                    aircrafts[tracknumber].Update(record);
                }
                this.recordIndex++;
            }
            //To do: remove aircrafts with 5 seconds+ unupdated
        }
    }

    class Aircraft
    {
        public ushort trackNumber;
        public string? id;
        public double latitude;
        public double longitude;
        public double height;
        public double? groundSpeed;
        public double? heading;
        public double lastUpdate;

        public Aircraft(CAT48 record)
        {
            this.trackNumber = record.TN;
            this.id = record.IDENTIFICATION;
            this.latitude = record.LATITUDE;
            this.longitude = record.LONGITUDE;
            this.groundSpeed = record.GS;
            this.heading = record.HEADING;
            this.lastUpdate = record.TIME;
        }

        public void Update(CAT48 record)
        {
            this.latitude = record.LATITUDE;
            this.longitude = record.LONGITUDE;
            this.groundSpeed = record.GS;
            this.heading = record.HEADING;
            this.lastUpdate = record.TIME;
        }
    }
}
