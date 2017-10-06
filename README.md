# Lambda Converters [![NuGet](https://img.shields.io/nuget/v/LambdaConverters.svg)](https://www.nuget.org/packages/LambdaConverters) [![ReSharper-Gallery](https://img.shields.io/badge/resharper--gallery-v3.0.0-lightgrey.svg)](https://resharper-plugins.jetbrains.com/packages/LambdaConverters.Annotations)

The library allows to create `IValueConverter`, `IMultiValueConverter`, `DataTemplateSelector`, and `ValidationRule` objects with the most convenient syntax available, ideally, using the lambda expressions.

## Lambda Value Converters

First create a (static) class and define your converters as static fields (or properties):

```csharp
internal static class Converters
{
    public static readonly IValueConverter VisibleIfTrue =
        ValueConverter.Create<bool, Visibility>(e => e.Value ? Visibility.Visible : Visibility.Collapsed);

    public static readonly IValueConverter VisibleIfNotNull =
        ValueConverter.Create<object, Visibility>(e => e.Value != null ? Visibility.Visible : Visibility.Collapsed);

    public static readonly IValueConverter ToUpperCase =
        ValueConverter.Create<string, string>(e => e.Value.ToUpper());
}
```

You're done! Just reference the converters with the `x:Static` expressions from your XAML files (assuming that `c` is the namespace definition for the `Converters` class):

```xml
<Button Visibility="{Binding model.IsAvailable, Converter={x:Static c:Converters.VisibleIfTrue}}" />

<TextBlock Text="{Binding model.Heading, Converter={x:Static c:Converters.ToUpperCase}}" />
```

### Features
- *strongly-typed* converters
- resource declaration not needed, just use the `x:Static` expressions
- separate class for each converter not needed anymore
- no redundant declarations: if you do not need the `ConvertBack` method, don't define it; otherwise, just put the second lambda expression
- full support for the remaining parameters of the `Convert` and `ConvertBack` methods: the `culture` and the `parameter` (also strongly-typed) are accessible as well
- if the conversion fails due to unexpected value types the optional [error strategy](Sources/LambdaConverters.Wpf/ConverterErrorStrategy.cs) can be specified

## Lambda Data Template Selectors

The library also allows to create `DataTemplateSelector` objects in the same convenient way as value converters. In order to define a selector simply write a static field (or property) similar to this snippet:

```csharp
internal static class TemplateSelector
{
    public static DataTemplateSelector AlternatingText =
        LambdaConverters.TemplateSelector.Create<int>(
            e => e.Item % 2 == 0
                ? (DataTemplate) ((FrameworkElement) e.Container)?.FindResource("BlackWhite")
                : (DataTemplate) ((FrameworkElement) e.Container)?.FindResource("WhiteBlack"));
}
```
Use your Lambda DataTemplateSelectors by referencing it with the `x:Static` markup extention (assuming that `s` is the namespace definition for the `TemplateSelector` class):

```xml
<UserControl.Resources>
    <DataTemplate x:Key="BlackWhite">
        <TextBlock Text="{Binding}" Foreground="Black" Background="White" />
    </DataTemplate>
    <DataTemplate x:Key="WhiteBlack">
        <TextBlock Text="{Binding}" Foreground="White" Background="Black" />
    </DataTemplate>
</UserControl.Resources>
<DockPanel>
    <ListBox ItemsSource="{Binding IntNumbers}"
             ItemTemplateSelector="{x:Static s:TemplateSelector.AlternatingText}">
    </ListBox>
</DockPanel>
```

Tada! All even numbers from `IntNumbers` are displayed with black font and white background and the odd numbers get the inverse font and background colors.

### Features
- *strongly-typed* Selectors
- resource declaration not needed, just use the `x:Static` expressions
- separate class for each selector not needed anymore
- full support for the remaining parameter `container`. For example, if you need to grab a `DataTemplate` from where the selector is use (see the example above).

## Lambda Validation Rules

Furthermore, you'll get Lambda ValidationRules on top. By now you know "the drill". First, define a `ValidationRule`object like this:

```csharp
public static class Rule
{
    public static ValidationRule IsNumericString =
        LambdaConverters.Validator.Create<string>(
            e => e.Value.All(char.IsDigit)
                ? ValidationResult.ValidResult
                : new ValidationResult(false, "Text has non-digit characters!"));
}
```
And then reference your new rule in vour `View` (assuming that `r` is the namespace definition for the `Rule` class):
```xml
<TextBox>
    <TextBox.Text>
        <Binding Path="Text" UpdateSourceTrigger="PropertyChanged">
            <Binding.ValidationRules>
                <x:Static Member="r:Rule.IsNumericString"/>
            </Binding.ValidationRules>
        </Binding>
    </TextBox.Text>
</TextBox>
```
Now, you made sure that only strings which consists of digits are passed to your `ViewModel`.

### Features
- *strongly-typed* rules
- resource declaration not needed, just use the `x:Static` expressions
- separate class for each rule not needed anymore
- full support for the remaining parameter `culture`

## Installation
Use the NuGet package manager to install the package.

:bulb: *ReSharper users*: use the Extension Manager to install the external annotations for the library.

## Limitations
The library currently supports the WPF only.

## Bugs? Questions? Suggestions?
Please feel free to [report them](https://github.com/michael-damatov/lambda-converters/issues).
