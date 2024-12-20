Imports System.IO
Imports DevExpress.Xpo
Imports DevExpress.Xpo.DB
Imports System.Linq
Imports DevExpress.Xpo.DB.Helpers

Friend Class Program

    Shared Sub Main(ByVal args() As String)

        'Pripojenie na lokalnu MySQL databazu
        Dim appDataPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        Dim connectionString As String = MySqlConnectionProvider.GetConnectionString("localhost", 3306, "root", "peter", "databaza")
        XpoDefault.DataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema)

        While True
            Console.Clear()
            Console.WriteLine(" ")
            Console.WriteLine("***************************")
            Console.WriteLine("********ZAMESTANNCI********")
            Console.WriteLine("**                       **")
            Console.WriteLine("** Prezeranie:        1  **")
            Console.WriteLine("** Hladanie:          2  **")
            Console.WriteLine("** Novy zaznam:       3  **")
            Console.WriteLine("** Uprava zaznamu:    4  **")
            Console.WriteLine("** Vymazat zaznam:    5  **")
            Console.WriteLine("** Vymazat vsetko:    6  **")
            Console.WriteLine("** Koniec:            7  **")
            Console.WriteLine("**                       **")
            Console.WriteLine("***************************")

            Dim volba As Integer = Console.ReadLine()

            Select Case volba
                Case 1
                    PrezeranieZaznamov()
                Case 2
                    HladanieZaznamu()
                Case 3
                    VkladanieUdajov()
                Case 4
                    UpravaZaznamu()
                Case 5
                    MazanieZaznamu()
                Case 6
                    MazanieVsetkychDat()
                Case 7
                    Console.WriteLine("...Koniec...")
                    End
                Case Else
                    Console.WriteLine("Nespravna volba, stlacte klaves pre pokracovanie")
                    Console.ReadKey()
            End Select
        End While

    End Sub

    Shared Sub VkladanieUdajov()
        'Zadanie udajov z klavesnice
        Console.WriteLine()
        Console.Write($"Zadaj meno a priezvysko: ")
        Dim Meno As String = Console.ReadLine()
        Console.Write($"Zadaj email: ")
        Dim Email As String = Console.ReadLine()
        Console.Write($"Zadaj telefonne cislo: ")
        Dim Telefon As String = Console.ReadLine()
        Console.Write($"Zadaj adresu: ")
        Dim Adresa As String = Console.ReadLine()

        'nacitanie udajov a ulozenie do tabulky 
        Using uow As New UnitOfWork()
            Dim newInfo As New Zamestnanci(uow)
            newInfo.Name = Meno
            newInfo.Email = Email
            newInfo.Phone = Telefon
            newInfo.Address = Adresa
            newInfo.Date = DateTime.Now
            uow.CommitChanges()
        End Using
        Console.WriteLine()
        Console.WriteLine("...Udaje ulozene, stlac klaves pre pokracovanie")
        Console.ReadKey()
    End Sub

    Shared Sub UpravaZaznamu()
        ' Uprava zaznamu - vyhladanie zaznamu a nastavenie jeho novych hodnot

        Console.WriteLine()
        Console.Write($"Zadaj Meno z tabulky 'Zamestnanci' pre ktore chcete upravit udaje: ")
        Dim HladanyZaznam As String = Console.ReadLine()

        Using uow As New UnitOfWork()
            ' Vyhladat zaznam podla mena
            Dim zaznam = uow.Query(Of Zamestnanci)().FirstOrDefault(Function(info) info.Name = HladanyZaznam)

            If zaznam Is Nothing Then
                Console.WriteLine("Zaznam nebol najdený.")
                Return
            End If

            ' Zobrazenie aktualnych hodnot
            Console.WriteLine($"Aktualne udaje: {zaznam.Name}, {zaznam.Email}, {zaznam.Phone}, {zaznam.Address}")

            ' Vstup novych hodnot
            Console.Write($"Zadaj nove meno a priezvisko (aktualne: {zaznam.Name}): ")
            Dim Meno As String = Console.ReadLine()
            Console.Write($"Zadaj novy email (aktualne: {zaznam.Email}): ")
            Dim Email As String = Console.ReadLine()
            Console.Write($"Zadaj nove telefonne cislo (aktualne: {zaznam.Phone}): ")
            Dim Telefon As String = Console.ReadLine()
            Console.Write($"Zadaj novu adresu (aktualne: {zaznam.Address}): ")
            Dim Adresa As String = Console.ReadLine()

            ' Aktualizacia hodnot
            zaznam.Name = If(String.IsNullOrEmpty(Meno), zaznam.Name, Meno)
            zaznam.Email = If(String.IsNullOrEmpty(Email), zaznam.Email, Email)
            zaznam.Phone = If(String.IsNullOrEmpty(Telefon), zaznam.Phone, Telefon)
            zaznam.Address = If(String.IsNullOrEmpty(Adresa), zaznam.Address, Adresa)
            zaznam.Date = DateTime.Now

            ' Ulozenie zmien
            uow.CommitChanges()
        End Using

        Console.WriteLine()
        Console.WriteLine("...upravene udaje ulozene, stlacte klaves pre pokracovanie")
        Console.ReadKey()
    End Sub


    Shared Sub PrezeranieZaznamov()
        'Citanie dat z tabulky 
        Console.WriteLine()
        Console.WriteLine($"Tabulka 'Zamestnanci' obsahuje nasledovne zaznamy: ")
        Console.WriteLine()
        Using uow As New UnitOfWork()
            Dim pocitadlo As Integer = 1
            Dim query = uow.Query(Of Zamestnanci)().OrderBy(Function(info) info.Date).Select(Function(info) $"{info.Name} {info.Email} {info.Phone} {info.Address}")
            For Each line In query
                Console.WriteLine(pocitadlo & ". " & line)
                pocitadlo += 1
            Next line
        End Using
        Console.WriteLine()
        Console.WriteLine("Stlac klaves pre pokracovanie")
        Console.ReadKey()
    End Sub

    Shared Sub HladanieZaznamu()
        'Hladanie  zaznamu
        Console.WriteLine()
        Console.Write($"Zadaj Meno z tabulky 'Zamestnanci' pre ktore chcete hladat udaje: ")
        Dim HladanyZaznam As String = Console.ReadLine()

        Using uow As New UnitOfWork()
            Dim query = uow.Query(Of Zamestnanci)().
            Where(Function(info) info.Name = HladanyZaznam).
            OrderBy(Function(info) info.Date).
            Select(Function(info) $"[{info.Date}] {info.Name} {info.Email} {info.Phone} {info.Address}")

            If query Is Nothing Then
                Console.WriteLine("Zaznam nebol najdeny.")
                Return
            End If

            For Each line In query
                Console.WriteLine(line)
            Next
        End Using
        Console.WriteLine()
        Console.WriteLine("Stlac klaves pre pokracovanie")
        Console.ReadKey()

    End Sub
    Shared Sub MazanieZaznamu()
        ' Mazanie zaznamu - vyhladanie zaznamu a nastavenie jeho novych hodnot

        Console.WriteLine()
        Console.Write($"Zadaj Meno z tabulky 'Zamestnanci' pre ktore chcete vymazat udaje: ")
        Dim HladanyZaznam As String = Console.ReadLine()

        Using uow As New UnitOfWork()
            ' Vyhladat zaznam podla mena
            Dim zaznam = uow.Query(Of Zamestnanci)().FirstOrDefault(Function(info) info.Name = HladanyZaznam)

            If zaznam Is Nothing Then
                Console.WriteLine("Zaznam nebol najdený.")
                Return
            End If

            ' Zobrazenie aktualnych hodnot
            Console.WriteLine($"Aktualne udaje: {zaznam.Name}, {zaznam.Email}, {zaznam.Phone}, {zaznam.Address}")

            uow.Delete(zaznam)
            uow.CommitChanges()

            Console.WriteLine($"Zaznam bol vymazany, stlac klaves pre pokracovanie")
            Console.ReadKey()

        End Using
    End Sub

    Shared Sub MazanieVsetkychDat()
        ' Mazanie dat:
        Using uow As New UnitOfWork()
            Dim itemsToDelete = uow.Query(Of Zamestnanci)().ToList()
            Console.WriteLine()
            Console.Write($"Pocet zaznamov v tabulke je: {itemsToDelete.Count}. Chcete vymazat VSETKY zaznamy (A/N)?: ")
            If Console.ReadLine().ToLowerInvariant() = "a" Then
                uow.Delete(itemsToDelete)
                uow.CommitChanges()
                Console.WriteLine($"Zaznamy vymazane, stlac klaves pre pokracovanie")
                Console.ReadKey()
            End If
        End Using
    End Sub

End Class


Public Class Zamestnanci
    Inherits XPLiteObject
    Public Sub New(ByVal session As Session)
        MyBase.New(session)
    End Sub
    'Key
    Private _key As Guid
    <Key(True)>
    Public Property Key() As Guid
        Get
            Return _key
        End Get
        Set(ByVal value As Guid)
            SetPropertyValue(NameOf(Key), _key, value)
        End Set
    End Property
    'Meno
    Private _name As String
    <Size(255)>
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            SetPropertyValue(NameOf(Name), _name, value)
        End Set
    End Property
    'Email
    Private _email As String
    <Size(255)>
    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            SetPropertyValue(NameOf(Email), _email, value)
        End Set
    End Property
    'Telefon
    Private _phone As String
    <Size(255)>
    Public Property Phone() As String
        Get
            Return _phone
        End Get
        Set(ByVal value As String)
            SetPropertyValue(NameOf(Phone), _phone, value)
        End Set
    End Property
    'Adresa
    Private _address As String
    <Size(255)>
    Public Property Address() As String
        Get
            Return _address
        End Get
        Set(ByVal value As String)
            SetPropertyValue(NameOf(Address), _address, value)
        End Set
    End Property
    'Datum
    Private _date As DateTime
    Public Property [Date]() As DateTime
        Get
            Return _date
        End Get
        Set(ByVal value As DateTime)
            SetPropertyValue(NameOf([Date]), _date, value)
        End Set
    End Property
End Class