using SpravaSkoleni;

SpravceSkoleni spravceSkoleni = new();
spravceSkoleni.NacistZamestnance();
spravceSkoleni.NacistSkoleni();

ZobrazVyberMenu();

void ZobrazVyberMenu()
{
    char volba;
    do
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n=== MENU ===");
        Console.WriteLine("1. Nové školení");
        Console.WriteLine("2. Seznam školení");
        Console.WriteLine("3. čeká na implementaci");
        Console.WriteLine("4. čeká na implementaci");
        Console.WriteLine("5. čeká na implementaci");
        Console.WriteLine("9. Výpis Ladící info");
        Console.WriteLine("0. Ukončit program");
        Console.ResetColor();
        Console.Write("\nVyber možnost: ");

        volba = Console.ReadKey().KeyChar;
        Console.WriteLine();

        // Zpracování volby uživatele
        switch (volba)
        {
            case '1': 
                spravceSkoleni.NoveSkoleni(); 
                break;

            case '2':
                Skoleni? vybraneSkoleni = spravceSkoleni.VybratSkoleni();
                if (vybraneSkoleni != null)
                {                    
                    spravceSkoleni.ZobrazDetailSkoleni(vybraneSkoleni);
                    if (vybraneSkoleni.Uzavreno)
                    {
                        Console.WriteLine("! Školení je uzavřené a nelze s ním provést žádnou operaci." + 
                            "\nPro návrat zpět stiskněte libovolnou klávesu !");
                        Console.ReadKey();
                        break;
                    }
                    else
                    {
                        char volbaDetailSkoleni;
                        do
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("\n=== MENU ÚPRAVY ŠKOLENÍ ===");
                            Console.WriteLine("1. Přihlásit studenta");
                            Console.WriteLine("2. Odhlásit studenta");
                            Console.WriteLine("3. Přiřadit lektora");
                            Console.WriteLine("4. Odebrat lektora");
                            Console.WriteLine("5. Uzavřít školení");                            
                            Console.WriteLine("6. Návrat do hlavního menu");
                            Console.ResetColor();
                            Console.Write("\nVyber možnost: ");

                            volbaDetailSkoleni = Console.ReadKey().KeyChar;
                            Console.WriteLine();
                            
                            switch (volbaDetailSkoleni)
                            {
                                case '1':                                    
                                    spravceSkoleni.PrihlasitStudenta(vybraneSkoleni);
                                    break;

                                case '2':                                    
                                    spravceSkoleni.OdhlasitStudenta(vybraneSkoleni);
                                    break;

                                case '3':                                    
                                    spravceSkoleni.PriraditLektora(vybraneSkoleni);
                                    break;

                                case '4':                                    
                                    spravceSkoleni.OdebratLektora(vybraneSkoleni);
                                    break;

                                case '5':                                    
                                    spravceSkoleni.UzavritSkoleni(vybraneSkoleni);
                                    break;

                                case '6':
                                    break;

                                default: 
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Neplatná volba, opakujte zadání:"); 
                                Console.ResetColor();
                                break;
                            }
                        }
                        while (volbaDetailSkoleni != '6');
                    }                                        
                }                
                break;

            case '3': 
                // čeká na implementaci 
                break;

            case '4': 
                // čeká na implementaci 
                break;

            case '5': 
                // čeká na implementaci 
                break;

            case '9': 
                spravceSkoleni.VypisLadiciInfo(); 
                break;

            case '0':                 
                break;

            default: 
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Neplatná volba, opakujte zadání:"); 
                Console.ResetColor();
                break;
        }

    } 
    while (volba != '0');
}