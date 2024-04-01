using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Options
{
    public class YandexDiskOptions
    {
        public string Key { get; set; } = null!;

        public string BasePath { get; set; } = null!;
    }
}