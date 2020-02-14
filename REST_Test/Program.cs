using System;
using McMaster.Extensions.CommandLineUtils;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CLI_Test
{
    [HelpOption("--hlp")]
    [Subcommand(
        //1
        typeof(List_),
        typeof(Add_),
        typeof(Update_),
        typeof(Delete_),
        typeof(Clear_),
        typeof(Done_),
        typeof(unDone_)
    )]
    
    class Program
    {
        static  async  Task<int> Main(string[] args)
        {
            return CommandLineApplication.Execute<Program>(args);
        }
    }
    public class Todo
    {
        public int id { get; set; }
        public string activity { get; set; }
        public bool condition { get; set; }
    }

   

    [Command(Description = "Show items in list", Name = "list")]
    class List_
    {
        public async Task OnExecuteAsync()
        {
            var Act = new List<string>();
            var client = new HttpClient();
            var result = await client.GetStringAsync("http://localhost:3000/todo");
            var Jsonn = JsonConvert.DeserializeObject<List<Todo>>(result);
            foreach (var X in Jsonn)
            {
                String Condition = null;
                if (X.condition == true)
                {
                    Condition = "(DONE)";
                }
                Act.Add($"{X.id}. {X.activity} {Condition}");
            }
            Console.WriteLine(String.Join("\n",Act));
        }
    }

    [Command(Description = "Add items in list", Name = "add")]
    class Add_
    {
        [Argument(0)]
        public string text { get; set; }
        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var AddText = new Todo() { activity = text, condition = false };
            var data = new StringContent(JsonConvert.SerializeObject(AddText), Encoding.UTF8, "application/json");
            await client.PostAsync("http://localhost:3000/todo", data);
        }
    }

    [Command(Description = "Update item in list", Name = "update")]
    class Update_
    {
        [Argument(0)]
        public string ID { get; set; }
        [Argument(1)]
        public string text { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var request = new { id = Convert.ToInt32(ID), activity = text };
            var data = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            await client.PatchAsync($"http://localhost:3000/todo/{ID}", data);
        }
    }

    [Command(Description = "Delete item in list", Name = "delete")]
    class Delete_
    {
        [Argument(0)]
        public string ID { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var request = new { id = Convert.ToInt32(ID) };
            await client.DeleteAsync($"http://localhost:3000/todo/{ID}");
        }
    }

    [Command(Description = "Delete all item in list", Name = "clear")]
    class Clear_ 
    {

        public async Task OnExecuteAsync()
        {
            var idList = new List<int>();
            var prompt = Prompt.GetYesNo("Are you sure?", false, ConsoleColor.Red);
            var client = new HttpClient();
            if (prompt)
            {
                var Activities = await client.GetStringAsync("http://localhost:3000/todo");
                var Jsonn = JsonConvert.DeserializeObject<List<Todo>>(Activities);
                foreach (var X in Jsonn)
                {
                    idList.Add(X.id);
                }
                foreach (var X in idList)
                {
                    await client.DeleteAsync($"http://localhost:3000/todo/{X}");
                }
            }
        }
    }
    [Command(Description = "set item in list to DONE", Name = "done")]
    class Done_
    {
        [Argument(0)]
        public string ID { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var request = new { id = Convert.ToInt32(ID) , condition = true };
            var data = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            await client.PatchAsync($"http://localhost:3000/todo/{ID}",data);
        }
    }
    [Command(Description = "set item in list to NOt DONE", Name = "undone")]
    class unDone_
    {
        [Argument(0)]
        public string ID { get; set; }

        public async Task OnExecuteAsync()
        {
            var client = new HttpClient();
            var request = new { id = Convert.ToInt32(ID), condition = false };
            var data = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            await client.PatchAsync($"http://localhost:3000/todo/{ID}", data);
        }
    }

}
//json-server --watch db.json
//{
//  "todo": [
//    {
//      "id": 4,
//      "activity": "Jogging jam 5.00 WIB",
//      "condition": false
//    },
//    {
//      "id": 3,
//      "activity": "Lari jam 7.00 WIB",
//      "condition": false
//    },
//    {
//      "id": 5,
//      "activity": "berenang di sawah",
//      "condition": false
//    },
//    {
//      "id": 6,
//      "activity": "Tidur siang",
//      "condition": false
//    }
//  ]
//}