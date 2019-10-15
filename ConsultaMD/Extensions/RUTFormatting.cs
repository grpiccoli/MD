using System.Globalization;

namespace ConsultaMD.Extensions
{
    public static class RUT
    {
        public static string DV(int rut)
        {
            int Id = rut;
            int Digito;
            int Contador;
            int Multiplo;
            int Acumulador;
            string RutDigito;

            Contador = 2;
            Acumulador = 0;

            while (Id != 0)
            {
                Multiplo = (Id % 10) * Contador;
                Acumulador += Multiplo;
                Id /= 10;
                Contador += 1;
                if (Contador == 8)
                {
                    Contador = 2;
                }
            }
            Digito = 11 - (Acumulador % 11);
            RutDigito = Digito.ToString().Trim();
            if (Digito == 10)
            {
                RutDigito = "K";
            }
            if (Digito == 11)
            {
                RutDigito = "0";
            }
            return (RutDigito);
        }
        public static string Format(int rut)
        {
            return $"{rut.ToString("N0", new CultureInfo("es-CL"))}-{DV(rut)}";
        }
        public static (int rut, string dv)? Unformat(string formatted)
        {
            var array = formatted.Replace(".", "").Split("-");
            var parsed = int.TryParse(array[0], out int rut);
            if (parsed)
            {
                if(DV(rut) == array[1].ToUpper()) return (rut, dv: array[1]);
            }
            return null;
        }
    }
}
