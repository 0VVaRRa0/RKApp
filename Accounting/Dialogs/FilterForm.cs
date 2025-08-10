namespace Accounting.Dialogs;

public class FilterForm : Form
{
    private Size windowSize = new(340, 192);
    public FilterForm()
    {
        Text = "Фильтр";
        Size = windowSize;
    }
}
