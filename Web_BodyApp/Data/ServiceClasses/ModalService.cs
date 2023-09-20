using System;
using System.Threading.Tasks;

public class ModalService
{
    public event Func<int, int?, DateTime?, Task> OnShow;
    public event Func<Task> OnClose;

    public async Task ShowAsync(int inputType, int? mealTimeDate, DateTime? entryDateTime)
    {
        await OnShow?.Invoke(inputType, mealTimeDate, entryDateTime);
    }

    public async Task Close() => await OnClose?.Invoke();
}
