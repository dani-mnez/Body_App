namespace Web_BodyApp.Data.ServiceClasses
{
    public class ReloadService
    {
        public event Func<Task> OnReloadAsync;
        public event Action OnReload;

        public async Task ReloadAsync() => await OnReloadAsync?.Invoke();

        public void Reload() => OnReload?.Invoke();
    }
}
