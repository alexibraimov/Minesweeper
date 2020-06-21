using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public class Program
    {
        static void Main()
        { //коммент 
            var form = new ParametersForm();
            Application.Run(form);
            if (form.Ok)
            {
                Map myMap = new Map(form.SizeMap, form.SizeMap.Height);
                myMap.GenerateMap();
                ProcessedMap processedMap = new ProcessedMap(myMap);
                Application.Run(new GameForm(processedMap));
            }
        }
    }
}
