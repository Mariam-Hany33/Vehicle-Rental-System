using System;
using System.Numerics;

using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Win32;
using System.Xml.Serialization;
using System.Threading;

namespace Vehicle_Rental_System

{
    class Customer        // Class for customers.
    {
        private string customerId;
        private string name;
        private string licenseNumber;
        private List<Rental> currentRentals;
        //______________________________________
        public Customer()        // constructor for set an initial value 
        {
            name = ReadName();
            customerId = ReadCustomerId();

            licenseNumber = ReadLicenseNumber();
            currentRentals = new List<Rental>();

        }
        //_______________________________

        private string ReadCustomerId()  // function to set value for Id.
        {

            while (true)
            {
                Console.Write("Enter Customer ID : ");
                try
                {
                    string id = Console.ReadLine();
                    if (id.Length > 0 && id.Length == 14)
                    {
                        return id;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("ID must be a positive number and 14 Digits . ");
                        Console.ResetColor();
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("invalid input!");
                    Console.ResetColor();

                }

            }

        }
        //_______________________________________

        private string ReadName()  // function to set value for name.
        {

            while (true)
            {
                Console.Write("Enter Customer Name : ");
                string name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Name!");
                    Console.ResetColor();
                }
            }
        }
        //____________________________________-
        private string ReadLicenseNumber()    // function to set value for License .
        {
            while (true)
            {
                Console.Write("Enter License Number : ");
                string LicenseNumber = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(LicenseNumber))
                {
                    return LicenseNumber;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("License Number cannot be Empty!");
                    Console.ResetColor();
                }
            }

        }
        //________________________________________________________
        // function for request rental.
        public void RequestRental(List<Vehicle> v, Rental[] r, int startDay, int startMonth, int startYear, int endDay, int endMonth, int endYear)
        {

            bool f = true;
            for (int i = 0; i < v.Count; i++)
            {
                if (v[i].IsAvailable() == true)
                {
                    r[i].setcode(v[i].vehicleId);

                    r[i].setcustomerid(this.customerId.ToString());
                    r[i].setrentalld("R" + new Random().Next(1000, 9999));
                    r[i].getrentalld();

                    r[i].SetStartDate(startDay, '/', startMonth, '/', startYear);
                    r[i].SetEndDate(endDay, '/', endMonth, '/', endYear);

                    v[i].status = "rented";
                    // r.Add(r[i]);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Rental created to {name}(ID {customerId})");
                    Console.WriteLine($"Vehicle {v[i].makeModel},from ({startDay}/{startMonth}/{startYear}) to ({endDay}/{endMonth}/{endYear})");
                    Console.ResetColor();
                    f = false;
                    break;
                }
            }
            if (f)
            {
                Console.WriteLine("No vehcile is available . ");
            }

        }
        //_____________________________________________
        //function to return vehicle.
        public void ReturnVehicle(List<Vehicle> v, Rental[] r, int actualDay, int acualMonth, int acualYear, string rentalled)
        {
            int index = 0;
            for (int i = 0; i < r.Length; i++)
            {
                if (rentalled == r[i].rentalld)
                {
                    index = i;
                    break;
                }
            }
            r[index].SetactualDate(actualDay, '/', acualMonth, '/', acualYear);
            v[index].status = "available";

            bool late = r[index].isLate();
            r[index].calculateCharge(late, v, index);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Vehicle {v[index].makeModel} returned successfully by {name}.");
            Console.ResetColor();
        }

    }



    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    public class Vehicle    // class for vehicle.
    {
        public string vehicleId;
        public string makeModel;
        public string type;
        public string status;
        public double dailyRate;
        public int lastServiceDay;
        public int lastServiceMonth;
        public int lastServiceYear;
        //************************
        public int availableDay;
        public int availableMonth;
        public int availableYear;
        public int latecost;

        //______________________________________
        public int setlatecost()    //function to set value for late cost.
        {
            string latecost;

            while (true)
            {
                bool f = false;
                latecost = Console.ReadLine();
                for (int i = 0; i < latecost.Length; i++)
                {
                    if (latecost[i] < '0' || latecost[i] > '9')
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("please enter valid fine .");
                        Console.ResetColor();
                        f = true;
                        break;


                    }
                }
                if (f == false)
                {
                    this.latecost = int.Parse(latecost);
                    return this.latecost;

                }

            }
        }
        //___________________________________


        public void setVehicleId(string code)  //function to set value for vehicle Id.
        {
            this.vehicleId = code;

        }
        //____________________________________
        public void setmakeModel(string model)  //function to set value for make model.
        {
            this.makeModel = model;

        }
        //-----------------------------------
        public void settype(string type)   //function to set value for type.
        {

            this.type = type;
        }
        //__________________________________-
        public void setstatus(string status)  //function to set value for type.
        {

            this.status = status;
        }
        //_______________________________________
        public bool setdailyRate(string dailyRate)   //function to set value for daily rate.
        {
            for (int i = 0; i < dailyRate.Length; i++)
            {
                if (char.IsDigit(dailyRate[i]) == false && dailyRate[i] != ',')
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter valid number .");
                    Console.ResetColor();
                    return false;
                }

            }
            this.dailyRate = double.Parse(dailyRate);
            return true;
        }
        //___________________________________________
        public bool setlastdate(int day, int month, int year)  //function to set value for last date.
        {


            this.lastServiceDay = day;
            this.lastServiceMonth = month;
            this.lastServiceYear = year;
            return true;
        }
        //_______________________________________________
        public bool setavailable(string day, string month, string year)  //function to set value for  available date. 
        {
            if (int.Parse(day) > 30 || int.Parse(month) > 12 || int.Parse(year) > 2025)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("please enter valid date.");
                Console.ResetColor();
                return false;
            }
            for (int i = 0; i < day.Length; i++)
            {
                if (char.IsDigit(day[i]) == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter valid date .");
                    Console.ResetColor();
                    return false;
                }
            }
            for (int i = 0; i < month.Length; i++)
            {
                if (char.IsDigit(month[i]) == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter valid date .");
                    Console.ResetColor();
                    return false;
                }
            }
            for (int i = 0; i < year.Length; i++)
            {
                if (char.IsDigit(year[i]) == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter valid date .");
                    Console.ResetColor();
                    return false;
                }
            }

            this.availableDay = int.Parse(day);
            this.availableMonth = int.Parse(month);
            this.availableYear = int.Parse(year);
            return true;
        }
        //______________________________________________


        public void Display()   //function to Display the vehicles.
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine();
            Console.WriteLine("                        ** Vehicle Details **");
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("                             Vehicle Id: " + "   ----->" + vehicleId);
            Console.WriteLine("                             Model: " + "        ----->" + makeModel);
            Console.WriteLine("                             Type: " + "         ----->" + type);
            Console.WriteLine("                             Status: " + "       ----->" + status);
            Console.WriteLine("                             Daily Rate: " + "   ----->" + dailyRate);
            Console.WriteLine("                             Last Service: " + " ----->" + lastServiceDay + "/" + lastServiceMonth + "/" + lastServiceYear);
        }
        //________________________________________________

        public bool MarkMaintenance(string d, string m, string y) //function to Mark Maintenance. 
        {
            if (int.Parse(d) > 30 || int.Parse(m) > 12 || int.Parse(y) < 2025)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("please enter valid date . ");
                Console.ResetColor();
                return false;
            }
            for (int i = 0; i < d.Length; i++)
            {
                if (d[i] < '0' || d[i] > '9')
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter valid date .");
                    Console.ResetColor();
                    return false;
                }
            }
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i] < '0' || m[i] > '9')
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter valid date .");
                    Console.ResetColor();
                    return false;
                }
            }
            for (int i = 0; i < y.Length; i++)
            {
                if (y[i] < '0' || y[i] > '9')
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter valid date .");
                    Console.ResetColor();
                    return false;
                }
            }

            status = "maintenance";
            lastServiceDay = int.Parse(d);
            lastServiceMonth = int.Parse(m);
            lastServiceYear = int.Parse(y);
            return true;



        }
        //____________________________________________

        public bool IsAvailable() //function to check the vehicle is available or not.
        {
            if (status == "available")
            {
                Display();
                return true;
            }
            else
                return false;
        }
    }
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    public class admain     //class for admain.
    {
        private string userName;
        private string password;
        //***************************************

        public admain()   //constructor for set an initial value.
        {
            userName = "";
            password = "";
        }
        //____________________________________________
        public bool SetUserName(string x)  //function to set value for user name.
        {
            if (x.Length < 8)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Name Must be more than 8 chars ");
                Console.ResetColor();
                return false;
            }
            else
            {
                userName = x;
                return true;
            }
        }
        //_______________________________________
        public bool SetPassword(string y)//function to set value for password.
        {
            if (y.Length >= 8)
            {
                password = y;
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("please enter password has at least 8 chars");
                Console.ResetColor();
                return false;
            }
        }
        //_______________________________________________
        public bool LogIn(string x, string y, int index)  //function to log in.
        {
            if (x == userName)
            {
                if (y == password)
                {

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($" ****** WELCOME {userName}******");
                    Console.WriteLine($"Your turn : {index} ");
                    Console.ResetColor();
                    Console.WriteLine();
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter valid password ");
                    Console.ResetColor();
                    return false;
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("please enter valid user name ");
                if (y != password)
                {

                    Console.WriteLine("please enter valid password ");

                }
                Console.ResetColor();

                return false;
            }

        }
        //_________________________________________
        //function to Add vehicle.
        public void AddVehicle(Vehicle x, string code, string model, string type, string status, string availableday, string availablemonth, string availableyear, string dailyRate, int lastServiceday, int lastServicemonth, int lastServiceyear)
        {

            x.setVehicleId(code);
            x.setmakeModel(model);
            x.settype(type);
            x.setstatus(status);
            x.setavailable(availableday, availablemonth, availableyear);
            x.setdailyRate(dailyRate);
            x.setlastdate(lastServiceday, lastServicemonth, lastServiceyear);
        }
        //_________________________________________
        public void GetVehicle(Vehicle x) //function to Display the vehicles.
        {

            Console.WriteLine($"Model :{x.makeModel} \nCode : {x.vehicleId} \nType : {x.type}\nStatus : {x.status}\nDaily rate : {x.dailyRate}\nLast service date : {x.lastServiceDay}/{x.lastServiceMonth}/{x.lastServiceYear}");

        }
        //______________________________________
        public void available(List<Vehicle> cars, int day, int month, int year)//function to check the vehicle is available or not. 
        {

            bool f = false;
            for (int i = 0; i < cars.Count; i++)
            {
                if (cars[i].status == "available" && cars[i].availableDay == day && cars[i].availableMonth == month && cars[i].availableYear == year)
                {
                    cars[i].Display();
                    f = true;
                }


            }
            if (f == false)
            {
                Console.WriteLine("Sorry , No car is available");
            }
        }
        //_______________________________
        // function to Veiw Maintenance Alerts.
        public bool VeiwMaintenanceAlerts(List<Vehicle> cars, int thisday, int thismonth, int thisyear)
        {

            for (int i = 0; i < cars.Count; i++)
            {
                int day = Math.Abs(cars[i].lastServiceDay - thisday) + Math.Abs(cars[i].lastServiceMonth - thismonth) * 30 + Math.Abs(cars[i].lastServiceYear - thisyear) * 12 * 30;
                if (day > 90)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Vehicle {cars[i].makeModel} needs maintenance . ");
                    Console.ResetColor();
                    cars[i].setlastdate(thisday, thismonth, thisyear);

                }


            }

            return true;
        }

    }
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    public class Rental    //class for rental.
    {
        public string rentalld;
        private string code;
        private string customerid;
        //**********************
        private int startdays;
        private int startmonth;
        private int startyear;
        //***********************
        private int endday;
        private int endmonth;
        private int endyear;
        //**********************
        private int actualday;
        private int actualmonth;
        private int actualyear;
        //*****************


        public Rental()    //constructor to set an initial value.
        {
            rentalld = "";
            code = "";
            customerid = "";
            //**********************
            startdays = 0;
            startmonth = 0;
            startyear = 0;
            //***********************
            endday = 0;
            endmonth = 0;
            endyear = 0;
            //**********************
            actualday = 0;
            actualmonth = 0;
            actualyear = 0;



        }
        //______________________________________
        public void setrentalld(string x)   //function to set a value for rentalled.
        {
            this.rentalld = x;
        }

        public void getrentalld()
        {
            Console.WriteLine("Your code : " + this.rentalld);

        }

        //__________________________
        public void setcode(string x)   ////function to set a value for code.
        {
            this.code = x;
        }
        //___________________________
        public bool setcustomerid(string x)  //function to set a value for customer Id.
        {
            if (x.Length != 14)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter valid ID ");
                Console.ResetColor();
                return false;

            }
            else
            {
                bool f = false;
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] >= '0' && x[i] <= '9')
                    {
                        f = false;

                    }
                    else
                    {
                        f = true;
                        break;
                    }
                }
                if (f)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Please enter valid ID ");
                    Console.ResetColor();
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }
        //_____________________________________________
        public bool SetStartDate(int day, char a, int month, char b, int year)  //function to set the  start date.
        {

            this.startdays = day;
            this.startmonth = month;
            this.startyear = year;
            return true;




        }
        //_________________________________________-

        public bool SetEndDate(int day, char a, int month, char b, int year)  ////function to set the end date.
        {

            this.endday = day;
            this.endmonth = month;
            this.endyear = year;
            return true;



        }
        //__________________________________________
        public bool SetactualDate(int day, char a, int month, char b, int year)  ////function to set the actual date.
        {


            this.actualday = day;
            this.actualmonth = month;
            this.actualyear = year;
            return true;



        }
        //_________________________________________

        public void calculateCharge(bool late, List<Vehicle> x, int index)//function to calclate charge.
        {
            int day;
            if (late == false)
            {
                int days = endday - startdays;
                int months = endmonth - startmonth;
                int year = endyear - startyear;
                day = days + months * 30 + year * 12 * 30;
                Console.WriteLine($"The cost : {day * x[index].dailyRate}");

            }
            else
            {
                int days = actualday - endday;
                int months = actualmonth - endmonth;
                int year = actualyear - endyear;
                day = days + months * 30 + year * 12 * 30;
                Console.WriteLine($"The cost and fine for delay: {day * x[index].latecost + x[index].dailyRate}");

            }

        }
        //_______________________________________________
        public bool isLate()  ////function to check the customer is late or not.
        {
            if (this.endday == this.actualday && this.endmonth == this.actualmonth && this.endyear == this.actualyear)
            {
                return false;
            }
            else if (this.endday > this.actualday && this.endmonth == this.actualmonth && this.endyear == this.actualyear)
            {
                return false;
            }
            else
            {
                return true;
            }




        }




    }
    //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    class Program
    {
        static void WelcomePage()     //welcome page.
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("                         ====================================");
            Console.WriteLine("                         ------> Welcome to Our System <------ ");
            Console.WriteLine("                         ====================================");
            Console.WriteLine();
            Console.WriteLine(" Press any key to continue ...");
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
        }
        //______________________________________
        static void ShowMenu()   // function to show a menu.
        {

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("                                 ========== Vehicle Rental System ==========");
            Console.WriteLine("                                 ║     1. Add Vehicle                      ║");
            Console.WriteLine("                                 ║     2. View All Vehicles                ║");
            Console.WriteLine("                                 ║     3. View the maintenance of vehicles.║");
            Console.WriteLine("                                 ║     4. Check Vehicle Availability       ║");
            Console.WriteLine("                                 ║     5. Exit                             ║");
            Console.WriteLine("                                 ===========================================");
            Console.ResetColor();
        }
        //_______________________________________
        static void showmenu2()// function to show a menu.
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("           =========================             ");
            Console.WriteLine("           ║        1- admain.     ║             ");
            Console.WriteLine("           ║        2- customer.   ║              ");
            Console.WriteLine("           =========================              ");
            Console.ResetColor();

        }
        //____________________________________
        static void showmenu3()// function to show a menu.
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("      ===========================     ");
            Console.WriteLine("      ║      1- To sign in .    ║     ");
            Console.WriteLine("      ║      2- To log in .     ║     ");
            Console.WriteLine("      ===========================     ");
            Console.ResetColor();

        }
        //________________________________
        static void showmenu4()// function to show a menu.
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("    ======================    ");
            Console.WriteLine("    ║    1- To rent .    ║    ");
            Console.WriteLine("    ║    2- To return .  ║    ");
            Console.WriteLine("    ======================     ");
            Console.ResetColor();
        }
        //_________________________________
        static void AddVehicle()// function to Add vehicle.
        {
            Vehicle car = new Vehicle();
            Console.Write("Enter Vehicle Id: ");
            car.vehicleId = Console.ReadLine();

            Console.Write("Enter Model: ");
            car.makeModel = Console.ReadLine();
            while (true)
            {
            ReadModel:
                if (car.makeModel.All(char.IsLetter))
                {
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("car model must contain letters, Try again.");
                    Console.ResetColor();
                    car.makeModel = Console.ReadLine();
                    goto ReadModel;
                }
            }

            Console.Write("Enter Type (Car/Bike/Van/Bus): ");
            car.type = Console.ReadLine();
            while (true)
            {
            Readtype:
                if (car.type.ToLower() == "car" || car.type.ToLower() == "bike" || car.type.ToLower() == "van" || car.type.ToLower() == "bus")
                {
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("type must be Car , Bike , Van , Bus .. Try Again");
                    Console.ResetColor();
                    car.type = Console.ReadLine();
                    goto Readtype;
                }
            }

            Console.Write("Enter Status (available/rented): ");
            car.status = Console.ReadLine();

            while (true)
            {
            Readstatus:
                if (car.status.ToLower() == "available" || car.status.ToLower() == "rented")
                {
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("type must be available , rented .. Try Again");
                    Console.ResetColor();
                    car.status = Console.ReadLine();
                    goto Readstatus;
                }
            }

            while (true)
            {
                Console.Write("Enter Daily Rate: ");
                string Rate = Console.ReadLine();
                if (double.TryParse(Rate, out double rate) && rate > 0)
                {
                    car.dailyRate = rate;
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Daily Rate must be a positive number... Try again");
                    Console.ResetColor();
                }
            }
            Console.Write("Enter Late cost : ");
            car.setlatecost();

            while (true)
            {
                Console.Write("Enter Last Service Day: ");
                string day = Console.ReadLine();
                if (int.TryParse(day, out int Day) && Day > 0 && Day < 32)
                {
                    car.lastServiceDay = Day;
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Last service Day must between 1 & 31... Try again");
                    Console.ResetColor();
                }
            }

            while (true)
            {
                Console.Write("Enter Last Service Month: ");
                string month = Console.ReadLine();
                if (int.TryParse(month, out int Month) && Month > 0 && Month < 13)
                {
                    car.lastServiceMonth = Month;
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Last service Month must between 1 & 12... Try again");
                    Console.ResetColor();
                }
            }

            while (true)
            {
                Console.Write("Enter Last Service Year: ");
                string year = Console.ReadLine();
                if (int.TryParse(year, out int Year) && Year >= 2000 && Year <= 2025)
                {
                    car.lastServiceYear = Year;
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Last service Year must between 2000 & 2025... Try again");
                    Console.ResetColor();
                }
            }
            vehicles.Add(car);
            Console.WriteLine("               Vehicle Added Successfully     ");
        }
        //____________________________________

        public static int checknum()   //function to check the number is valid or not.
        {
        f:
            int x;
            try
            {

                x = int.Parse(Console.ReadLine());

            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("please enter valid number.");
                Console.ResetColor();
                goto f;

            }
            return x;

        }
        //______________________________________
        static void DisplayAll()   //function to display the vehicles.
        {
            if (vehicles.Count == 0)
            {
                Console.WriteLine("No vehicles found!");
                return;
            }

            foreach (var v in vehicles)
            {
                v.Display();
            }
        }
        //________________________________________
        public static int checkchoice(int l, int r) //function to check the choice in renge or not.
        {
            int x = 0;
            bool e = false;
            while (e != true)
            {

                x = checknum();
                if (x >= l && x <= r)
                {
                    e = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("please enter valid choice . ");
                    Console.ResetColor();
                }

            }
            return x;

        }
        //_____________________________________

        public static int checkday()  // function to check the day in range or not .

        {
            int d;
        s:
            try
            {
                d = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The Day Must between 1 && 31 ....Try again .");
                Console.ResetColor();
                goto s;
            }
            if (d > 31)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The Day Must between 1 && 31 ....Try again .");
                Console.ResetColor();
                goto s;
            }

            return d;


        }
        //____________________________________________
        public static int checkmonth()  // function to check the month in range or not .

        {
            int d;
        s:
            try
            {
                d = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The Month Must between 1 && 12 ....Try again .");
                Console.ResetColor();
                goto s;
            }
            if (d > 12)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The Month Must between 1 && 12 ....Try again .");
                Console.ResetColor();
                goto s;
            }

            return d;




        }
        //_________________________________________
        public static int checkYear()// function to check the year in range or not .

        {
            int d;
        s:
            try
            {
                d = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The Year Must be more than 2024 ....Try again .");
                Console.ResetColor();
                goto s;
            }
            if (d < 2024)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The Year Must be more than 2024 ....Try again .");
                Console.ResetColor();
                goto s;
            }

            return d;


        }
        //___________________________________________



        static void available(List<Vehicle> vehicles, int d, int m, int y)// function to check the vehicles is available or not.
        {
            bool f = false;
            for (int i = 0; i < vehicles.Count; i++)
            {
                if (vehicles[i].status == "available" && vehicles[i].availableDay == d && vehicles[i].availableMonth == m && vehicles[i].availableYear == y)
                {
                    vehicles[i].Display();
                    f = true;
                }


            }
            if (f == false)
            {
                Console.WriteLine("Sorry , No car is available");
            }

        }
        //@@@@@@@@@@@@@@ Main @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        static List<Vehicle> vehicles = new List<Vehicle>();
        static Rental[] rentals = new Rental[20000];
        static void Main(string[] args)
        {
            WelcomePage();
            admain[] admains = new admain[20000];

            Customer[] customers = new Customer[200000];

            for (int i = 0; i < 20000; i++)
            {
                rentals[i] = new Rental();
                admains[i] = new admain();


            }
            //_____________________________________
            for (int i = 0; i < 20000; i++)
            {

                showmenu2();

                Console.Write("Your choice : ");
                int choice = checkchoice(1, 2);

                switch (choice)
                {

                    case 1:

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Hello ,our dear admain.");
                        showmenu3();
                        Console.WriteLine("Your choice : ");
                        int choice2 = checkchoice(1, 2);

                        if (choice2 == 1)
                        {

                            Console.Write("please enter username : ");
                            string user = "";
                            do
                            {
                                user = Console.ReadLine();

                            } while (admains[i].SetUserName(user) != true);
                            Console.Write("please enter password : ");
                            string password = "";
                            do
                            {
                                password = Console.ReadLine();

                            } while (admains[i].SetPassword(password) != true);



                            string user1;
                            string password1;

                            do
                            {
                                Console.Write("please enter username to login : ");
                                user1 = Console.ReadLine();
                                Console.Write("please enter your password to login : ");
                                password1 = Console.ReadLine();
                            } while (admains[i].LogIn(user1, password1, i) != true);



                        }
                        else
                        {
                            Console.Write("please enter your number : ");
                            int num = checknum();
                            num--;
                            string user1;
                            string password1;

                            do
                            {
                                Console.Write("please enter username to login : ");
                                user1 = Console.ReadLine();
                                Console.Write("please enter your password to login : ");
                                password1 = Console.ReadLine();
                            } while (admains[i].LogIn(user1, password1, i) != true);



                        }
                    Start:
                        ShowMenu();
                        Console.Write("Your Choice : ");
                        int q = checkchoice(1, 4);

                        switch (q)
                        {
                            case 1:
                                AddVehicle();
                                break;
                            case 2:
                                DisplayAll();
                                break;
                            case 3:
                                Console.Write("This Day : ");
                                int d = checkday();
                                Console.Write("This Month : ");
                                int m = checkmonth();
                                Console.Write("This Yeard : ");
                                int y = checkYear();
                                admains[i].VeiwMaintenanceAlerts(vehicles, d, m, y);
                                break;
                            case 4:
                                Console.Write("The Day You want to rent : ");
                                int day = checkday();
                                Console.Write("The Month You want to rent : ");
                                int month = checkmonth();
                                Console.Write("The Year You want to rent : ");
                                int year = checkYear();
                                available(vehicles, day, month, year);
                                break;
                            case 5:
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("Exiting program... BYE");
                                Console.ResetColor();
                                return;
                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Invalid choice .. Please try again");
                                Console.ResetColor();
                                break;
                        }
                        break;

                    case 2:
                        Console.WriteLine("go case 2");
                        //  rentals[i] = new Rental();

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Hello ,our dear customer .");
                        Console.ResetColor();
                        Console.WriteLine();

                        customers[i] = new Customer();
                        showmenu4();
                        Console.Write("Your choice : ");
                        int choice3 = checkchoice(1, 2);

                        if (choice3 == 1)
                        {

                            int startday = 0, startmonth = 0, startyear = 0;
                            int endday = 0, endmonth = 0, endyear = 0;



                            Console.Write("The Day you want to rent : ");
                            startday = checkday();
                            Console.Write("The Month you want to rent : ");
                            startmonth = checkmonth();
                            Console.Write("The Year you want to rent : ");
                            startyear = checkYear();
                            rentals[i].SetStartDate(startday, '/', startmonth, '/', startyear);


                            Console.Write("The Day you want to return : ");
                            endday = checkday();
                            Console.Write("The Day you want to return : ");
                            endmonth = checkmonth();
                            Console.Write("The Day you want to return : ");
                            endyear = checkYear();
                            rentals[i].SetEndDate(endday, '/', endmonth, '/', endyear);

                            customers[i].RequestRental(vehicles, rentals, startday, startmonth, startyear, endday, endmonth, endyear);


                        }
                        else
                        {
                            int index = 0;
                            Console.Write("Please enter your code : ");
                        Code:
                            bool f = false;
                            string code = Console.ReadLine();
                            for (int p = 0; p < vehicles.Count; p++)
                            {
                                if (code == rentals[p].rentalld)
                                {
                                    index = p;
                                    f = true;
                                    break;
                                }


                            }
                            if (f == false)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("No Vehicle has this Id ..... Try again.");
                                Console.ReadLine();
                                goto Code;

                            }
                            Console.Write("The Day you return this vehicle : ");
                            int d = checkday();
                            Console.Write("The month you return this vehicle : ");
                            int m = checkmonth();
                            Console.Write("The Year you return this vehicle : ");
                            int y = checkYear();
                            customers[i].ReturnVehicle(vehicles, rentals, d, m, y, vehicles[index].vehicleId);

                        }
                        break;
                }
            }

        }
    }
}





