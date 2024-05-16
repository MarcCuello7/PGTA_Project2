using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Project2_Code
{
    class AsterixSimulation
    {
        public double simSpeed;
        public int recordIndex;
        public double time;
        public List<CAT48> CAT48list;
        public Dictionary<ushort, Aircraft> aircrafts;
        
        public AsterixSimulation(AsterixParser parser)
        {
            this.simSpeed = 1;
            this.recordIndex = 0;
            this.aircrafts = new Dictionary<ushort, Aircraft>();
            this.CAT48list = parser.CAT48list;
            this.CAT48list = this.CAT48list.OrderBy(o => o.TIME).ToList();
            this.time = Math.Floor(this.CAT48list[0].TIME);
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
            
            //fix remove aircrafts not only on stale
            var staleAircrafts = aircrafts.Where(a => this.time - a.Value.lastUpdate >= 10).ToArray();
            foreach (var a in staleAircrafts)
            {
                aircrafts.Remove(a.Key);
            }                
        }

        public void Reset()
        {
            this.time = Math.Floor(this.CAT48list[0].TIME);
            this.simSpeed = 1;
            this.recordIndex = 0;
            this.aircrafts.Clear();
        }
    }

    class Aircraft
    {
        public ushort trackNumber;
        public string id;
        public double latitude;
        public double longitude;
        public double height;
        public double flightLevel;
        public double groundSpeed;
        public double heading;
        public double lastUpdate;
        public byte type;

        public Aircraft(CAT48 record)
        {
            this.trackNumber = record.TN;
            this.id = record.IDENTIFICATION;
            this.latitude = record.LATITUDE;
            this.longitude = record.LONGITUDE;
            this.height = record.HEIGHT;
            this.flightLevel = record.FL ?? -1;
            this.groundSpeed = record.GS ?? -1;
            this.heading = record.HEADING ?? -1;
            this.lastUpdate = record.TIME;
            this.type = record.TYP_020 ?? 0;
        }

        public void Update(CAT48 record)
        {
            this.latitude = record.LATITUDE;
            this.longitude = record.LONGITUDE;
            this.height = record.HEIGHT;
            this.flightLevel = record.FL ?? this.flightLevel;
            this.groundSpeed = record.GS ?? this.groundSpeed;
            this.heading = record.HEADING ?? this.heading;
            this.lastUpdate = record.TIME;
        }
    }
}
