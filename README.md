# <img src="Icon.png" width="48" height="48" /> Lambda Converters

The library allows to write `IValueConverter` and `IMultiValueConverter` objects with the most convenient syntax available, ideally, using the lambda expressions.

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

## Features
- *strongly-typed* converters
- resource declaration not needed, just use the `x:Static` expressions
- separate class for each converter not needed anymore
- no redundant declarations: if you do not need the `ConvertBack` method, don't define it; otherwise, just put the second lambda expression
- full support for the remaining parameters of the `Convert` and `ConvertBack` methods: the `culture` and the `parameter` (also strongly-typed) are accessible as well
- if the conversion fails due to unexpected value types the optional [error strategy](Sources/LambdaConverters.Wpf/ConverterErrorStrategy.cs) can be specified

:bulb: *ReSharper users*: use the Extension Manager to install the external annotations for the library.

## Installation
Use the NuGet package manager to install the package.

## Limitations
The library currently supports the WPF only.

## Bugs? Questions? Suggestions?
Please feel free to [report them](https://github.com/michael-damatov/lambda-converters/issues).
