module Common =
  type ID = int
  let pdgAll = System.Windows.Forms.Padding(6)
  let lclAll = System.Globalization.CultureInfo.InvariantCulture
  let dbConnString =
    "Data Source=YAWAR-PC;
    Initial Catalog=TSQLFundamentals2008;
    Integrated Security=SSPI;"

  module View =
    open System.Data
    open System.Data.SqlClient
    open System.Windows.Forms

    type BoundComboBox(connString: string, qry: string) as this =
      inherit ComboBox()

      let _datSuppliers = new DataTable(Locale = lclAll)
      let _bscSuppliers =
        new BindingSource(DataSource = _datSuppliers)
      let _sdaSuppliers = new SqlDataAdapter(qry, connString)

      do
        this.DataSource <- _bscSuppliers
        _sdaSuppliers.Fill(_datSuppliers) |> ignore
        this.AutoCompleteMode <- AutoCompleteMode.SuggestAppend
        this.AutoCompleteSource <- AutoCompleteSource.ListItems

module ProductQuery =
  module Model =
    type t =
      { SupplierID: Common.ID
        CategoryID: Common.ID
        ShowDiscontinued: bool }

  module View =
    open System.Drawing
    open System.Windows.Forms

    let mutable private _model: option<Model.t> = None
    let model () = _model

    let root (): Control =
      let _root =
        new FlowLayoutPanel(Padding = Common.pdgAll, AutoSize = true)

      let lblSupplier =
        new Label(
          Text = "Supplier:",
          TextAlign = ContentAlignment.BottomRight,
          AutoSize = true,
          Parent = _root
        )
      let cmbSupplier =
        new Common.View.BoundComboBox(
          Common.dbConnString, 
          "select supplierid as ID, companyname as [Name]
          from Production.Suppliers"
        )
      cmbSupplier.Parent <- _root
      cmbSupplier.DisplayMember <- "Name"
      cmbSupplier.ValueMember <- "ID"

      let lblCategory =
        new Label(
          Text = "Category:",
          TextAlign = ContentAlignment.BottomRight,
          AutoSize = true,
          Parent = _root
        )
      let cmbCategory =
        new Common.View.BoundComboBox(
          Common.dbConnString,
          "select categoryid as ID, categoryname as [Name]
          from Production.Categories"
        )
      cmbCategory.Parent <- _root
      cmbCategory.DisplayMember <- "Name"
      cmbCategory.ValueMember <- "ID"

      let lblShowDiscontinued =
        new Label(
          Text = "Show discontinued:",
          TextAlign = ContentAlignment.BottomRight,
          AutoSize = true,
          Parent = _root
        )
      let chkShowDiscontinued =
        new CheckBox(Parent = _root, AutoSize = true)

      let btnGetInfo = new Button(Text = "Get Info", Parent = _root)
      btnGetInfo.Click.Add(
        fun _ ->
          _model <-
            Some
              { SupplierID = cmbSupplier.SelectedValue :?> Common.ID
                CategoryID = cmbCategory.SelectedValue :?> Common.ID
                ShowDiscontinued = chkShowDiscontinued.Checked }
      )

      // Upcast required because `_root` is of a derived type of
      // System.Windows.Forms.Control.
      _root :> Control

module Main =
  open System
  open System.Drawing
  open System.Windows.Forms

  let main () =
    Application.EnableVisualStyles()
    let f = new Form(Text = "Product Query", Size = new Size(800, 600))
    let pnlMain =
      new FlowLayoutPanel(
        FlowDirection = FlowDirection.TopDown,
        Parent = f,
        AutoSize = true
      )

    pnlMain.Controls.Add(ProductQuery.View.root ())
    Application.Run(f)

  [<STAThread>]
  do main ()

