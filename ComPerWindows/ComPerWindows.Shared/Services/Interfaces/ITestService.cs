using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ComPerLibrary.Models;

namespace ComPerWindows.Services.Interfaces
{
    public interface ITestService
    {
        Task Start();

        Task<SmallObjectTestModel> PostSmallHttpAsync(CancellationToken token);
        Task<LargeObjectTestModel> PostLargeHttpAsync(CancellationToken token);

        Task<SmallObjectTestModel> PostSmallStreamAsync(CancellationToken token);

        Task<LargeObjectTestModel> PostLargeStreamAsync(CancellationToken token);
    }
}
