using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using ComPerLibrary.Models;
using ComPerWindows.Common.Commands;
using ComPerWindows.Common.LifeCycle;
using ComPerWindows.Services.Interfaces;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json.Linq;

namespace ComPerWindows.ViewModels.Pages
{
    public class TestPageViewModel : BasePageViewModel
    {
        #region Fields

        private readonly ITestService _testService;
        private readonly IStorageService _storageService;
        private bool _isStarted = false;

        private ConcurrentQueue<SmallObjectTestModel> SmallStreamModelsQueue;
        private ConcurrentQueue<SmallObjectTestModel> SmallHttpModelsQueue;
        private ConcurrentQueue<LargeObjectTestModel> LargeStreamModelsQueue;
        private ConcurrentQueue<LargeObjectTestModel> LargeHttpModelsQueue;

        private List<SmallObjectTestModel> SmallStreamModelsList;
        private List<SmallObjectTestModel> SmallHttpModelsList;
        private List<LargeObjectTestModel> LargeStreamModelsList;
        private List<LargeObjectTestModel> LargeHttpModelsList; 

        #endregion

        #region Properties

        public AwaitableDelegateCommand GetSmallStreamCommand { get; set; }

        public AwaitableDelegateCommand GetLargeStreamCommand { get; set; }

        public AwaitableDelegateCommand GetSmallHttpCommand { get; set; }

        public AwaitableDelegateCommand GetLargeHttpCommand { get; set; }

        public AwaitableDelegateCommand SaveCommand { get; set; }

        #endregion

        public TestPageViewModel(ITestService testService, IStorageService storageService)
        {
            _testService = testService;
            _storageService = storageService;

            GetSmallStreamCommand = new AwaitableDelegateCommand(ExecutePostSmallStreamCommand);
            GetLargeStreamCommand = new AwaitableDelegateCommand(ExecutePostLargeStreamCommand);
            
            GetSmallHttpCommand = new AwaitableDelegateCommand(ExecutePostSmallHttpCommand);
            GetLargeHttpCommand = new AwaitableDelegateCommand(ExecutePostLargeHttpCommand);

            SaveCommand = new AwaitableDelegateCommand(ExecuteSaveAndClearAllLists);

            SmallStreamModelsList = new List<SmallObjectTestModel>();
            SmallHttpModelsList = new List<SmallObjectTestModel>();
            LargeStreamModelsList = new List<LargeObjectTestModel>();
            LargeHttpModelsList = new List<LargeObjectTestModel>();

            SmallStreamModelsQueue = new ConcurrentQueue<SmallObjectTestModel>();
            SmallHttpModelsQueue = new ConcurrentQueue<SmallObjectTestModel>();
            LargeStreamModelsQueue = new ConcurrentQueue<LargeObjectTestModel>();
            LargeHttpModelsQueue = new ConcurrentQueue<LargeObjectTestModel>();

            Task.Run(async () =>
            {
                while (true)
                {
                    SmallObjectTestModel objectInQueue;
                    if (SmallStreamModelsQueue.TryDequeue(out objectInQueue))
                    {
                        lock (SmallStreamModelsList)
                        {
                            SmallStreamModelsList.Add(objectInQueue);
                        }
                    }
                    else
                    {
                        await Task.Delay(200);
                    }
                }
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    SmallObjectTestModel objectInQueue;
                    if (SmallHttpModelsQueue.TryDequeue(out objectInQueue))
                    {
                        lock (SmallHttpModelsList)
                        {
                            SmallHttpModelsList.Add(objectInQueue);
                        }
                    }
                    else
                    {
                        await Task.Delay(200);
                    }
                }
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    LargeObjectTestModel objectInQueue;
                    if (LargeStreamModelsQueue.TryDequeue(out objectInQueue))
                    {
                        lock (LargeStreamModelsList)
                        {
                            LargeStreamModelsList.Add(objectInQueue);
                        }
                    }
                    else
                    {
                        await Task.Delay(200);
                    }
                }
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    LargeObjectTestModel objectInQueue;
                    if (LargeHttpModelsQueue.TryDequeue(out objectInQueue))
                    {
                        lock (LargeHttpModelsList)
                        {
                            LargeHttpModelsList.Add(objectInQueue);
                        }
                    }
                    else
                    {
                        await Task.Delay(200);
                    }
                }
            });
        }

        #region Command Methods

        private async Task PostLargeStream()
        {
            var result = await _testService.PostLargeStreamAsync(CancellationToken.None);
            LargeStreamModelsQueue.Enqueue(result);
        }

        private async Task PostSmallStream()
        {
            var result = await _testService.PostSmallStreamAsync(CancellationToken.None);
            SmallStreamModelsQueue.Enqueue(result);
        }

        private async Task PostLargeHttp()
        {
            var result = await _testService.PostLargeHttpAsync(CancellationToken.None);
            LargeHttpModelsQueue.Enqueue(result);
        }

        private async Task PostSmallHttp()
        {
            var result = await _testService.PostSmallHttpAsync(CancellationToken.None);
            SmallHttpModelsQueue.Enqueue(result);
        }

        private int numberOfTests = 100;

        private async Task ExecutePostLargeStreamCommand()
        {
            await StartIfNotStartedAsync();

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < numberOfTests; i++)
            {
                tasks.Add(PostLargeStream());
            }

            await Task.WhenAll(tasks);
        }

        private async Task ExecutePostSmallStreamCommand()
        {
            await StartIfNotStartedAsync();

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < numberOfTests; i++)
            {
                tasks.Add(PostSmallStream());
            }

            await Task.WhenAll(tasks);
        }

        private async Task ExecutePostLargeHttpCommand()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < numberOfTests; i++)
            {
                tasks.Add(PostLargeHttp());
            }

            await Task.WhenAll(tasks);
        }

        private async Task ExecutePostSmallHttpCommand()
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < numberOfTests; i++)
            {
                tasks.Add(PostSmallHttp());
            }

            await Task.WhenAll(tasks);
        }

        private async Task ExecuteSaveAndClearAllLists()
        {
            while (!SmallHttpModelsQueue.IsEmpty && !SmallStreamModelsQueue.IsEmpty && !LargeHttpModelsQueue.IsEmpty &&
                   !LargeStreamModelsQueue.IsEmpty)
            {

            }

            await _storageService.CreateOrUpdateData("SmallHttpModelsList", SmallHttpModelsList);
            await _storageService.CreateOrUpdateData("SmallStreamModelsList", SmallStreamModelsList);
            await _storageService.CreateOrUpdateData("LargeHttpModelsList", LargeHttpModelsList);
            await _storageService.CreateOrUpdateData("LargeStreamModelsList", LargeStreamModelsList);

            Debug.WriteLine("------");
            Debug.WriteLine("SmallHttpModelsList: {0}", JToken.FromObject(SmallHttpModelsList));
            Debug.WriteLine("------");
            Debug.WriteLine("SmallStreamModelsList: {0}", JToken.FromObject(SmallStreamModelsList));
            Debug.WriteLine("------");
            Debug.WriteLine("LargeHttpModelsList: {0}", JToken.FromObject(LargeHttpModelsList));
            Debug.WriteLine("------");
            Debug.WriteLine("LargeStreamModelsList: {0}", JToken.FromObject(LargeStreamModelsList));
            Debug.WriteLine("------");
        }

        private async Task StartIfNotStartedAsync()
        {
            if (!_isStarted)
            {
                await _testService.Start();
                _isStarted = true;
            }
        }

        #endregion

        #region Navigation Methods

        public override void LoadState(LoadStateEventArgs e)
        {
            base.LoadState(e);
        }

        public override void SaveState(SaveStateEventArgs e)
        {
            base.SaveState(e);
        }

        #endregion
    }
}
