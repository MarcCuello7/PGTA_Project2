# ASTERIX DECODER README

Hello, welcome to our ASTERIX Decoder, a PGTA project focused on developing an ASTERIX decoder for category 048. This document is mainly directed at PGTA teachers, it contains the project planning and a brief user guide.

The language used to program the project is C# in Visual Studio, utilizing its Windows Presentation Foundation technology and the GMAP.NET library for the graphical component of the work.

The developed program is a decoder for ASTERIX files of CAT048. It has two main parts: the decoder, which reads the data of a binary file and decode it into an understandable table format, and the simulation, which shows the information decoded graphically as aircrafts on a map.

## User guide

As you open the application, you will see everything disabled but the _Load asterix file_ button located inside a menu called _File_ at the top left of the window. So the first thing you will have to do is upload an .ast file using the file search window that will appear after pressing the button. Then, wait for the file to load (the longest ones can take close to 1 minute).

After loading the file you will see that all the features have been enabled. At the top left of the window there are two tabs, the radar and the table. 

Starting with the radar, it has the objective to show you the aircrafts location in a more graphically way. Here you have four main features:

   - The _Play_ button, it will start the simulation and change into the  _Pause_ button which will stop the simulation.
   - The _Speed_ slider which let you select a value between 1 and 100 corresponding to the simulation speed you want (e.g. if you put this value at 100, for every second in reality one hundred seconds will pass in the simulation).
   - The _Time_ slider shows you the actual time of the simulation and let you advance or delay the simulation to a desired time.
   - The _Reset_ button resets the simulation to its initial values: simulation paused, speed value to 1 and time to its starter.

While you are using this radar you can also see some information of the aircrafts (identification, ground speed, height, latitude and longitude) if you put your mouse over them in the map.

On the other hand, in the table tab, you will see a table with all the data contained in the file in a readible way. Every row is a record and every column is a different piece of data of that record (index, coordinates, heigh, data items, etc.).

In addition, if you go to the second menu called _Filters_, you will be able to select between three different filters (fixed transponder, pure white and flights on ground) or a combination of them and filter the data depending on your selections. In the simulation, the filtered aircrafts will disappear of the radar and in the table, the rows corresponding to the records of the filtered aircrafts will also disappear.

Finally, using the button _Export to CSV_ located on the _File_ menu, you will be able to export all the decoded data in your asterix file into a CSV to facilitate future analysis (project 3). Note that if you export to CSV while having selected filters, the generated CSV file will also have those filters applied.

## Planning

### https://github.com/MarcCuello7/PGTA_Project2

Since I have done the project alone, there was no clear planning or distribution of the project. Thanks to the teacher sessions every week I knew the order I had to follow in order to complete the work, so I followed those steps and after each session I committed the changes in the GitHub repository. Based on this I will make an estimation of the planning I followed:

  - **Apr 9, 2024 (8h)**: FileParser class created and half done, CAT48 class created and added all Data Items variables, ParseFSPEC and first Data Items done and implemented some of the functions of a binary reader library into the Utils class.
  - **Apr 11, 2024 (5h)**: FileParser ended and organized CAT48 class with List<Actions> of all Data Items.
  - **Apr 21, 2024 (6h)**: Done some Data Items parser functions.
  - **Apr 24, 2024 (5h)**: Almost all Data Items parser functions done and implemented two's component function on Utils.
  - **Apr 28, 2024 (8h)**: All decodification finished and prepared and informed about graphical interface (WPF).
  - **Apr 29, 2024 (3h)**: Added GMap.NET library, invetigated its functionality and edited its strating values.
  - **May 1, 2024 (7h)**: Added GeoUtils library and converted rho and theta into geodesian coordinates.
  - **May 2, 2024 (8h)**: Added Simulation and Aircraft class to use them on the graphical part.
  - **May 6, 2024 (5h)**: Fixed some errors with coordinates, added mouse over information on aircrafts and removed bugged aircrafts on the simulation. 
  - **May 7, 2024 (6h)**: Simulaion ditails cleaned (zoom and heading), added play/pause button and added controlled section on the WPF.
  - **May 9, 2024 (8h)**: Created table tab on WPF and the data table with its respective strings on each data item. Fixed aircrafts figure.
  - **May 10, 2024 (8h)**: Optimized the program completely changing the way the data table is created, now uses dictionaries to write the strings. Simulation reset and export CSV buttons done.
  - **May 12, 2024 (6h)**: Added filter and export CSV functions and cleaned the WPF (pictures in buttons, positions, widths, etc.). Ordered correctly data items on the table.
  - **May 14, 2024 (3h)**: Decodified all BDS interns data items and printed coordinates as º ' '' on mouseover.
  - **May 15, 2024 (8h)**: Corrected BDS interns data wrong values, decodified mode3areplay and address (hexadecimal and octal) and added speed modification on the simulation.
  - **May 16, 2024 (5h)**: Added time slider and decodified ModeC_correcred.
  - **May 17, 2024 (6h)**: At present, finishing to write the README at 6am 18h before finish line. All memory to write yet.

Total time (estimated) invested in the project: **110h**.
