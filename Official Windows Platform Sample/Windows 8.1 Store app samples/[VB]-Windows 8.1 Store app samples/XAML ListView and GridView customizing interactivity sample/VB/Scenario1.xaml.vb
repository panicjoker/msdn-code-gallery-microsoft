'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports ListViewInteraction.Expression.Blend.SampleData.SampleDataSource
Imports Windows.Foundation

Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' represents the store data - See SampleData folder
    Private storeData As StoreData = Nothing

    Public Sub New()
        Me.InitializeComponent()

        ' initializes the store data
        storeData = New StoreData()
        ' sets the source of the GridView to the store data collection
        ItemGridView.ItemsSource = storeData.Collection
    End Sub

    Private Sub AddToCart_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.NotifyUser("", NotifyType.StatusMessage)
        ShoppingCart.Text = "Cart Contents: "
        Dim charsToTrim() As Char = {","c, " "c}

        If ItemGridView.SelectedItems.Count <> 0 Then
            For Each item As Item In ItemGridView.SelectedItems
                ShoppingCart.Text += item.Title & ", "
            Next item
            ShoppingCart.Text = ShoppingCart.Text.TrimEnd(charsToTrim)
            ShoppingCart.Text &= " added to cart"
        Else
            rootPage.NotifyUser("Please select items to place in the cart", NotifyType.ErrorMessage)
        End If
    End Sub


#Region "Data Visualization"
    ''' <summary>
    ''' We will visualize the data item in asynchronously in multiple phases for improved panning user experience 
    ''' of large lists.  In this sample scneario, we will visualize different parts of the data item
    ''' in the following order:
    ''' 
    '''     1) Placeholders (visualized synchronously - Phase 0)
    '''     2) Tilte (visualized asynchronously - Phase 1)
    '''     3) Category and Image (visualized asynchronously - Phase 2)
    '''
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub ItemGridView_ContainerContentChanging(ByVal sender As ListViewBase, ByVal args As ContainerContentChangingEventArgs)
        Dim iv As ItemViewer = TryCast(args.ItemContainer.ContentTemplateRoot, ItemViewer)

        If args.InRecycleQueue = True Then
            iv.ClearData()
        ElseIf args.Phase = 0 Then
            iv.ShowPlaceholder(TryCast(args.Item, Item))

            ' Register for async callback to visualize Title asynchronously
            args.RegisterUpdateCallback(ContainerContentChangingDelegate)
        ElseIf args.Phase = 1 Then
            iv.ShowTitle()
            args.RegisterUpdateCallback(ContainerContentChangingDelegate)
        ElseIf args.Phase = 2 Then
            iv.ShowCategory()
            iv.ShowImage()
        End If

        ' For imporved performance, set Handled to true since app is visualizing the data item
        args.Handled = True
    End Sub

    ''' <summary>
    ''' Managing delegate creation to ensure we instantiate a single instance for 
    ''' optimal performance. 
    ''' </summary>
    Private ReadOnly Property ContainerContentChangingDelegate() As TypedEventHandler(Of ListViewBase, ContainerContentChangingEventArgs)
        Get
            If _delegate Is Nothing Then
                _delegate = New TypedEventHandler(Of ListViewBase, ContainerContentChangingEventArgs)(AddressOf ItemGridView_ContainerContentChanging)
            End If
            Return _delegate
        End Get
    End Property
    Private _delegate As TypedEventHandler(Of ListViewBase, ContainerContentChangingEventArgs)

#End Region 'Data Visualization

End Class
