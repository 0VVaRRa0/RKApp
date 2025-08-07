namespace Accounting.Dialogs;

class ServiceEditorForm : Form
{
    Size windowSize = new(683, 384);
    public ServiceEditorForm()
    {
        Text = "Услуга";
        MinimumSize = windowSize;
        MaximumSize = windowSize;
    }
}
