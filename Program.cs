public class Program
{
    public static void Main(string[] args)
    {
        var donoTable = new Table<Dono>();
        var catTable = new Table<Cat>();

        string query = catTable.InnerJoin(donoTable,(Cat cat,Dono Dono)=> cat.IdDono == Dono.Id);


        Console.WriteLine(query);
    }
}