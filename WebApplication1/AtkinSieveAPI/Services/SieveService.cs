using System.Collections.Generic;

namespace SieveApp.Services
{
    public class SieveService
    {
        public List<int> SieveOfAtkin(int limit)
        {
            var primes = new List<int>();
            var isPrime = new bool[limit + 1];

            if (limit > 2) primes.Add(2);
            if (limit > 3) primes.Add(3);

            for (int x = 1; x * x <= limit; x++)
            {
                for (int y = 1; y * y <= limit; y++)
                {
                    int n = (4 * x * x) + (y * y);
                    if (n <= limit && (n % 12 == 1 || n % 12 == 5))
                        isPrime[n] ^= true;

                    n = (3 * x * x) + (y * y);
                    if (n <= limit && n % 12 == 7)
                        isPrime[n] ^= true;

                    n = (3 * x * x) - (y * y);
                    if (x > y && n <= limit && n % 12 == 11)
                        isPrime[n] ^= true;
                }
            }

            for (int r = 5; r * r <= limit; r++)
            {
                if (isPrime[r])
                {
                    for (int i = r * r; i <= limit; i += r * r)
                        isPrime[i] = false;
                }
            }

            for (int a = 5; a <= limit; a++)
            {
                if (isPrime[a])
                    primes.Add(a);
            }

            return primes;
        }
    }
}