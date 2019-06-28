using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Producer_Consumer
{
    static class Globals
    {
        public static readonly int PasswordLegth = 4;
        public static readonly char[] Alphabet = new char[] { 'q', 'w', 'e'}; 
        //public static readonly char[] Alphabet = new char[] { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p' };
        public static readonly int AlphabetLegth = Alphabet.Length;
        public static Buffer<Element<char[]>> GlobalBuffer = new Buffer<Element<char[]>>();

        public static string CharToString(char[] array)
        {
            string result = new string(array);

            return result; 
        }

        public static bool CharEquals(char[] array1, char[] array2)
        {
            if (array1.Length != array2.Length) return false; 

            for(int i=0; i<array1.Length; i++)
            {
                if (array1[i] != array2[i]) return false; 
            }

            return true; 
        }
    }


    class Element<T>
    {
        public T element { get; set; }
        public bool last = false;

        public Element(T element, bool last)
        {
            this.element = element;
            this.last = last; 
        }
    }

    class Buffer<T>
    {
        Queue<T> buffer_queue = new Queue<T>(); 

        public void add(T elemet)
        {
            buffer_queue.Enqueue(elemet); 
        }

        public T get()
        {
            return buffer_queue.Dequeue(); 
        }

        public bool IsNotEmpty()
        {
            return buffer_queue.Count != 0;  
        }
    }

    class Producer
    {
        public readonly int id; 
        char[] password = new char[Globals.PasswordLegth];

        public Producer(int id)
        {
            this.id = id; 
        }

        public void produce(int level)
        {
            if(level == Globals.PasswordLegth)
            {
                char[] array = new char[Globals.PasswordLegth];
                password.CopyTo(array,0); 
                Element<char[]> element = new Element<char[]>(array, false);
                Console.WriteLine("Producer {0} adding: {1}", id, Globals.CharToString(element.element));
                Globals.GlobalBuffer.add(element); 
            }

            else
            {
                for(int i=0; i<Globals.AlphabetLegth; i++)
                {
                    password[level] = Globals.Alphabet[i];
                    produce(level + 1);
                }
            }
        }

        public void produce()
        {
            produce(0);

            Element<char[]> element = new Element<char[]>(password, true);
            Globals.GlobalBuffer.add(element); 
        }

    }

    class Consumer
    {
        int id;
        char[] password = new char[Globals.PasswordLegth];

        public Consumer(int id, char[] password)
        {
            this.id = id;
            this.password = password; 
        }

        public bool consume()
        { 
            if (!Globals.GlobalBuffer.IsNotEmpty())
            {
                Console.WriteLine("Consumer {0} queue is empty: waiting...", id);
                return false; 
            }

            
            Element<char[]> element = Globals.GlobalBuffer.get();
            Console.WriteLine("Consumer {0} getting: {1}", id, Globals.CharToString(element.element));

            if (Globals.CharEquals(password, element.element))
            {
                Console.WriteLine("Consumer {0} found passwword: {1}", id, Globals.CharToString(element.element));
                return true;
            }

            if(element.last)
            {
                Console.WriteLine("Consumer {0} didn't find passwword", id);
                return true; 
            }

            else
            {
                return false; 
            }

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Aplhabet: {0}", Globals.CharToString(Globals.Alphabet));

            char[] password = new char[] { 'a', 'q', 'q', 'w' };  

            Producer producer = new Producer(0);
            Consumer consumer = new Consumer(0, password);
            Thread producerThread = new Thread(() => producer.produce()); 
            Thread consumerThread = new Thread(() => 
            {
                while (!consumer.consume()) ;
                producerThread.Abort();
                Console.WriteLine("Producer aborted");
            });

            Console.WriteLine("Producer started"); 
            producerThread.Start();
            Console.WriteLine("Cousumer started");
            consumerThread.Start();

            producerThread.Join();
            Console.WriteLine("Producer joined"); 
            consumerThread.Join();
            Console.WriteLine("Consumer joined"); 
        }
    }
}

