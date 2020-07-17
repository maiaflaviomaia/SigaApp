
$(document).ready(function () {
    $('.date').mask('11/11/1111');
    $('.time').mask('00:00');
    $('.date_time').mask('99/99/9999 00:00:00');
    $('.cep').mask('99999-999');
    $('.cpf').mask('000.000.000-00', { reverse: true });
    $('.cnpj').mask('00.000.000/0000-00', { reverse: true });
    $('.money').mask('000.000.000.000.000,00', { reverse: true });
    $('.telefone').mask('(99) 9999-9999');
    $('.celular').mask('(99) 99999-9999');
    $('.agencia').mask('0000-0', { reverse: true });
    $('.conta').mask('00000000-0', { reverse: true });
    $('.money2').mask("#.##0,00", { reverse: true });
    $('.percent').mask('###,00', { reverse: true });
    $('.valor').mask('###0.00', { reverse: true });
});

$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

var Itens = new Object();
Itens.Servicos = new Array();
Itens.Fornecedores = new Array();
Itens.CustosProducao = new Array();

//VARIÁVEIS GERAIS DO ORÇAMENTO
var bv = 0;
var lucro = 0;
var imposto = 0;
var comissoes = 0;
var acrescimos = 0;
var desconto = 0;
var subTotal = 0;
var subTotal2 = 0;
var totalOrcamento = 0;
var totalTaxas = 0;
var valorLucro = 0;
var valorBV = 0;
var valorImposto = 0;
var valorComissao = 0;

//VARIÁVEIS PARA ADICIONAR SERVIÇOS NA LISTA
var ServicoID = document.getElementById("servicoSelecionado");
var QtdServico = document.getElementById("txtQuantidade");
var ListaServico = document.getElementById("listaServicos");
var TotalServicos = document.getElementById("divTotal");
var txtTotalOrcamento = document.getElementById("txtTotalOrcamento");
var ServicoJSON = document.getElementById("listaServicosJSON");

//VARIÁVEIS PARA ADICIONAR FORNECEDORES NA LISTA
var FornecedorID = document.getElementById("fornecedorSelecionado");
var DescricaoForncedor = document.getElementById("txtDescricaoFornecedor");
var QuantidadeFornecedor = document.getElementById("txtQuantidadeFornecedor");
var ValorUnitarioFornecedor = document.getElementById("txtValorUnitarioForncedor");
var ListaFornecedor = document.getElementById("listaFornecedores");
var TotalFornecedores = document.getElementById("divTotalFornecedores");
var FornecedoresJSON = document.getElementById("listaFornecedoresJSON");

//VARIÁVEIS PARA ADICIONAR CUSTOS DE PRODUÇÃO NA LISTA
var DescricaoCusto = document.getElementById("txtDescricaoCusto");
var QuantidadeCusto = document.getElementById("txtQuantidadeCusto");
var UnidadeCusto = document.getElementById("txtUnidadeCusto");
var ValorUnitarioCusto = document.getElementById("txtValorUnitarioCusto");
var ListaCusto = document.getElementById("listaCustosProducao");
var TotalCustos = document.getElementById("divTotalProducao");
var CustoJSON = document.getElementById("listaCustoProducaoJSON");

function AdicionarServico() {

    var descricaoServico = ServicoID.options[ServicoID.selectedIndex].text;
    var arrayServicoPreco = descricaoServico.split('|');
    var total = (arrayServicoPreco[1].replace(",", ".") * QtdServico.value);
    total = Math.round(total * 100) / 100;

    Itens.Servicos.push({
        "ServicoPrestadoID": ServicoID.value,
        "Descricao": arrayServicoPreco[0],
        "Quantidade": QtdServico.value,
        "PrecoUnitario": arrayServicoPreco[1],
        "ValorTotal": total
    });

    ServicoJSON.innerHTML = JSON.stringify(Itens.Servicos);

    var servico = "<tr>" +
        "<td class='text-center'>" + ServicoID.value + "</td>" +
        "<td class='text-center'>" + arrayServicoPreco[0] + "</td>" +
        "<td class='text-center'>" + QtdServico.value + "</td>" +
        "<td class='text-center'>" + arrayServicoPreco[1] + "</td>" +
        "<td class='text-center valor-calculado-servico'>" + total + "</td>" +
        "<td class='text-center'><button type='button' class='btn btn-sm btn-danger' onclick = 'removerServico(event)'>Excluir</button></td>" +
        "</tr>";

    ListaServico.innerHTML += servico;

    TotalizarServico();
    SubTotalOrcamento();
    SubTotal2();
    LimparCampos();
}

function TotalizarServico() {
    var els = document.getElementsByClassName("valor-calculado-servico");
    var valorcalculado = 0;

    [].forEach.call(els, function (el) {
        valorcalculado = el.innerText * 1 + valorcalculado;
    });

    document.getElementById("divTotal").innerText = valorcalculado;
}


function AdicionarFornecedor() {
    var ValorTotalFornecedor = QuantidadeFornecedor.value * ValorUnitarioFornecedor.value;
    ValorTotalFornecedor = Math.round(ValorTotalFornecedor * 100) / 100;

    Itens.Fornecedores.push({
        "FornecedorID": FornecedorID.value,
        "Descricao": DescricaoForncedor.value,
        "Quantidade": QuantidadeFornecedor.value,
        "ValorUnitario": ValorUnitarioFornecedor.value,
        "ValorTotal": ValorTotalFornecedor
    });

    FornecedoresJSON.innerHTML = JSON.stringify(Itens.Fornecedores);

    var fornecedor = "<tr>" +
        "<td class='text-center'>" + FornecedorID.value + "</td>" +
        "<td class='text-center'>" + FornecedorID.options[FornecedorID.selectedIndex].text + "</td>" +
        "<td class='text-center'>" + DescricaoForncedor.value + "</td>" +
        "<td class='text-center'>" + QuantidadeFornecedor.value + "</td>" +
        "<td class='text-center'>" + ValorUnitarioFornecedor.value + "</td>" +
        "<td class='text-center valor-calculado-fornecedor'>" + ValorTotalFornecedor + "</td>" +
        "<td class='text-center'><button type='button' class='btn btn-sm btn-danger' onclick = 'removerFornecedor(event)'>Excluir</button></td>" +
        "</tr>";

    ListaFornecedor.innerHTML += fornecedor;

    TotalizarFornecedor();
    SubTotalOrcamento();
    SubTotal2();
    LimparCampos();
}

function TotalizarFornecedor() {
    var els = document.getElementsByClassName("valor-calculado-fornecedor");
    var valorcalculado = 0;

    [].forEach.call(els, function (el) {
        valorcalculado = el.innerText * 1 + valorcalculado;
    });

    document.getElementById("divTotalFornecedores").innerText = valorcalculado;
}

function AdicionarCustoProducao() {
    var ValorTotalCusto = QuantidadeCusto.value * ValorUnitarioCusto.value;
    ValorTotalCusto = Math.round(ValorTotalCusto * 100) / 100;

    Itens.CustosProducao.push({
        "Descricao": DescricaoCusto.value,
        "Quantidade": QuantidadeCusto.value,
        "ValorUnitario": ValorUnitarioCusto.value,
        "UnidadeValor": UnidadeCusto.value,
        "ValorTotal": ValorTotalCusto
    });

    CustoJSON.innerHTML = JSON.stringify(Itens.CustosProducao);

    var custo = "<tr>" +
        "<td class='text-center'>" + DescricaoCusto.value + "</td>" +
        "<td class='text-center'>" + QuantidadeCusto.value + "</td>" +
        "<td class='text-center'>" + ValorUnitarioCusto.value + "</td>" +
        "<td class='text-center'>" + UnidadeCusto.value + "</td>" +
        "<td class='text-center valor-calculado_custo'>" + ValorTotalCusto + "</td>" +
        "<td class='text-center'><button type='button' class='btn btn-sm btn-danger' onclick = 'removerCusto(event)'>Excluir</button></td>" +
        "</tr>";

    ListaCusto.innerHTML += custo;

    TotalizarCustos();
    SubTotalOrcamento();
    SubTotal2();
    LimparCampos();
}

function TotalizarCustos() {
    var els = document.getElementsByClassName("valor-calculado_custo");
    var valorcalculado = 0;

    [].forEach.call(els, function (el) {
        valorcalculado = el.innerText * 1 + valorcalculado;
    });

    document.getElementById("divTotalProducao").innerText = valorcalculado;
}

function LimparCampos() {
    $("#txtQuantidade").val("1");
    $("#txtDescricaoFornecedor").val("");
    $("#txtQuantidadeFornecedor").val("1");
    $("#txtValorUnitarioForncedor").val("");
    $("#txtDescricaoCusto").val("");
    $("#txtQuantidadeCusto").val("1");
    $("#txtUnidadeCusto").val("");
}

function removerServico(event) {
    var target = event.target;
    var parent = target.parentNode.parentNode;

    Itens.Servicos.splice(parent.rowIndex - 1, 1);
    ServicoJSON.innerHTML = JSON.stringify(Itens.Servicos);
    parent.remove();

    TotalizarServico();
    SubTotalOrcamento();
    SubTotal2();
    TotalizarOrcamento();
}

function removerFornecedor(event) {
    var target = event.target;
    var parent = target.parentNode.parentNode;

    Itens.Fornecedores.splice(parent.rowIndex - 1, 1);
    FornecedoresJSON.innerHTML = JSON.stringify(Itens.Fornecedores);
    parent.remove();

    TotalizarFornecedor();
    SubTotalOrcamento();
    SubTotal2();
    TotalizarOrcamento();
}

function removerCusto(event) {
    var target = event.target;
    var parent = target.parentNode.parentNode;

    Itens.CustosProducao.splice(parent.rowIndex - 1, 1);
    CustoJSON.innerHTML = JSON.stringify(Itens.CustosProducao);
    parent.remove();

    TotalizarCustos();
    SubTotalOrcamento();
    SubTotal2();
    TotalizarOrcamento();
}

function SubTotalOrcamento() {
    setTimeout(function () {
        var ts = TotalServicos.innerText;
        var tf = TotalFornecedores.innerText;
        var tc = TotalCustos.innerText;

        subTotal = parseFloat(ts) + parseFloat(tf) + parseFloat(tc);
        document.getElementById("txtSubTotal").innerText = subTotal.toLocaleString('pt-BR', { minimumFractionDigits: 2, style: 'currency', currency: 'BRL' });

        SubTotal2();
        TotalizarOrcamento();
    }, 100);
}


function SubTotal2() {
    setTimeout(function () {
        var sb = $('#txtLucro').val();

        if (sb == "") {
            sb = 0;
        }

        subTotal2 = (subTotal * (parseFloat(sb) / 100)) + subTotal;

        if (Number.isNaN(subTotal2)) {
            subTotal2 = 0;
        } else {
            subTotal2;
        }

        TotalizarOrcamento();
        document.getElementById("txtSubTotal2").innerText = subTotal2.toLocaleString('pt-BR', { minimumFractionDigits: 2, style: 'currency', currency: 'BRL' });
    }, 100);
}


function SomarTaxas() {
    setTimeout(function () {
        bv = $('#txtBV').val();
        imposto = $('#txtImposto').val();
        comissoes = $('#txtComissao').val();

        if (Number.isNaN(bv) || bv == "") {
            bv = 0;
        } else {
            bv;
        }

        if (Number.isNaN(imposto) || imposto == "") {
            imposto = 0;
        } else {
            imposto;
        }

        if (Number.isNaN(comissoes) || comissoes == "") {
            comissoes = 0;
        } else {
            comissoes;
        }

        totalTaxas = parseFloat(bv) + parseFloat(imposto) + parseFloat(comissoes);
        TotalizarOrcamento();
    }, 100);
}

function TotalizarOrcamento() {
    setTimeout(function () {
        var a = (subTotal2 / ((100 - parseFloat(totalTaxas)) / 100));
        var b = $('#txtAcrescimo').val().replace(".", "").replace(",", ".");
        var c = $('#txtDesconto').val().replace(".", "").replace(",", ".");

        console.log(a);
        console.log(b);
        console.log(c);

        if (Number.isNaN(a) || a == "") {
            a = 0;
        } else {
            a;
        }

        if (Number.isNaN(b) || b == "") {
            b = 0;
        } else {
            b;
        }

        if (Number.isNaN(c) || c == "") {
            c = 0;
        } else {
            c;
        }

        totalOrcamento = parseFloat(a) + parseFloat(b) - parseFloat(c);

        document.getElementById("divTotalOrcamento").innerText = totalOrcamento.toLocaleString('pt-BR', { minimumFractionDigits: 2, style: 'currency', currency: 'BRL' });
        MostrarValorLucro();
        MostrarValorBV();
        MostrarValorImposto();
        MostrarValorComissao();
        txtTotalOrcamento.value = totalOrcamento.toFixed(2).toString().replace(".", ",");
    }, 100);

}

function MostrarValorLucro() {
    setTimeout(function () {
        var a = $('#txtLucro').val().replace(",", ".");
        valorLucro = subTotal * (parseFloat(a) / 100);

        if (Number.isNaN(valorLucro)) {
            valorLucro = 0;
        } else {
            valorLucro;
        }

        document.getElementById("valorLucro").innerText = (parseFloat(valorLucro)).toLocaleString('pt-BR', { minimumFractionDigits: 2, style: 'currency', currency: 'BRL' });
    }, 100)
}

function MostrarValorBV() {
    setTimeout(function () {
        var b = $('#txtBV').val().replace(",", ".");
        valorBV = totalOrcamento * (parseFloat(b) / 100);

        if (Number.isNaN(valorBV)) {
            valorBV = 0;
        } else {
            valorBV;
        }

        document.getElementById("valorBV").innerText = (parseFloat(valorBV)).toLocaleString('pt-BR', { minimumFractionDigits: 2, style: 'currency', currency: 'BRL' });
    }, 100)
}

function MostrarValorImposto() {
    setTimeout(function () {
        var b = $('#txtImposto').val().replace(",", ".");
        valorImposto = totalOrcamento * (parseFloat(b) / 100);

        if (Number.isNaN(valorImposto)) {
            valorImposto = 0;
        } else {
            valorImposto;
        }

        document.getElementById("valorImposto").innerText = (parseFloat(valorImposto)).toLocaleString('pt-BR', { minimumFractionDigits: 2, style: 'currency', currency: 'BRL' });
    }, 100)
}

function MostrarValorComissao() {
    setTimeout(function () {
        var b = $('#txtComissao').val().replace(",", ".");
        valorComissao = totalOrcamento * (parseFloat(b) / 100);

        if (Number.isNaN(valorComissao)) {
            valorComissao = 0;
        } else {
            valorComissao;
        }

        document.getElementById("valorComissao").innerText = (parseFloat(valorComissao)).toLocaleString('pt-BR', { minimumFractionDigits: 2, style: 'currency', currency: 'BRL' });
    }, 100)
}

function MontarLayout() {
    var tipo = document.getElementById("tipoOrcamento").value;

    if (tipo == "Audio") {
        document.getElementById("statusOrc").style.display = 'none';
        document.getElementById("dataOrc").style.display = 'block';
        document.getElementById("dataVal").style.display = 'block';
        document.getElementById("cliente").style.display = 'block';
        document.getElementById("solicitante").style.display = 'block';
        document.getElementById("titulo").style.display = 'block';
        document.getElementById("nomePeca").style.display = 'none';
        document.getElementById("tipoVeiculacao").style.display = 'none';
        document.getElementById("pracaVeiculacao").style.display = 'none';
        document.getElementById("duracao").style.display = 'none';
        document.getElementById("periodoVeiculacao").style.display = 'none';
        document.getElementById("tipoMidia").style.display = 'none';
        document.getElementById("servico").style.display = 'block';
        document.getElementById("fornecedor").style.display = 'block';
        document.getElementById("custo").style.display = 'block';
        document.getElementById("observacoes").style.display = 'block';
        document.getElementById("blocoTotal").style.display = 'block';
    }

    if (tipo == "Video" || tipo == "Publicidade") {
        document.getElementById("statusOrc").style.display = 'none';
        document.getElementById("dataOrc").style.display = 'block';
        document.getElementById("dataVal").style.display = 'block';
        document.getElementById("cliente").style.display = 'block';
        document.getElementById("solicitante").style.display = 'block';
        document.getElementById("titulo").style.display = 'block';
        document.getElementById("nomePeca").style.display = 'block';
        document.getElementById("tipoVeiculacao").style.display = 'block';
        document.getElementById("pracaVeiculacao").style.display = 'block';
        document.getElementById("duracao").style.display = 'block';
        document.getElementById("periodoVeiculacao").style.display = 'block';
        document.getElementById("tipoMidia").style.display = 'block';
        document.getElementById("servico").style.display = 'block';
        document.getElementById("fornecedor").style.display = 'block';
        document.getElementById("custo").style.display = 'block';
        document.getElementById("observacoes").style.display = 'block';
        document.getElementById("blocoTotal").style.display = 'block';
    }

    if (tipo == "") {
        document.getElementById("statusOrc").style.display = 'none';
        document.getElementById("dataOrc").style.display = 'none';
        document.getElementById("dataVal").style.display = 'none';
        document.getElementById("cliente").style.display = 'none';
        document.getElementById("solicitante").style.display = 'none';
        document.getElementById("titulo").style.display = 'none';
        document.getElementById("nomePeca").style.display = 'none';
        document.getElementById("tipoVeiculacao").style.display = 'none';
        document.getElementById("pracaVeiculacao").style.display = 'none';
        document.getElementById("duracao").style.display = 'none';
        document.getElementById("periodoVeiculacao").style.display = 'none';
        document.getElementById("tipoMidia").style.display = 'none';
        document.getElementById("observacoes").style.display = 'none';
        document.getElementById("servico").style.display = 'none';
        document.getElementById("fornecedor").style.display = 'none';
        document.getElementById("custo").style.display = 'none';
        document.getElementById("observacoes").style.display = 'block';
        document.getElementById("blocoTotal").style.display = 'none';
    }
}

function MontarLayoutTitulos() {
    var tipo = document.getElementById("tipoLancamentoTitulo").value;

    if (tipo == 0) {
        document.getElementById("txtfornecedor").style.display = 'none';
        document.getElementById("txtcliente").style.display = 'none';
        document.getElementById("txtdescricao").style.display = 'none';
        document.getElementById("txtvalor").style.display = 'none';
        document.getElementById("txttipoDocumento").style.display = 'none';
        document.getElementById("txtnumeroDocumento").style.display = 'none';
        document.getElementById("txtehRecorrente").style.display = 'none';
        document.getElementById("txtehParcelado").style.display = 'none';
        document.getElementById("txtparcelas").style.display = 'none';
        document.getElementById("txtobservacoes").style.display = 'none';
        document.getElementById("txtbotoes").style.display = 'none';
    }

    if (tipo == 1) {
        document.getElementById("txtfornecedor").style.display = 'none';
        document.getElementById("txtcliente").style.display = 'block';
        document.getElementById("txtdescricao").style.display = 'block';
        document.getElementById("txtvalor").style.display = 'block';
        document.getElementById("txttipoDocumento").style.display = 'block';
        document.getElementById("txtnumeroDocumento").style.display = 'block';
        document.getElementById("txtehRecorrente").style.display = 'block';
        document.getElementById("txtehParcelado").style.display = 'block';
        document.getElementById("txtparcelas").style.display = 'none';
        document.getElementById("txtobservacoes").style.display = 'block';
        document.getElementById("txtbotoes").style.display = 'block';
    }

    if (tipo == 2) {
        document.getElementById("txtfornecedor").style.display = 'block';
        document.getElementById("txtcliente").style.display = 'none';
        document.getElementById("txtdescricao").style.display = 'block';
        document.getElementById("txtvalor").style.display = 'block';
        document.getElementById("txttipoDocumento").style.display = 'block';
        document.getElementById("txtnumeroDocumento").style.display = 'block';
        document.getElementById("txtehRecorrente").style.display = 'block';
        document.getElementById("txtehParcelado").style.display = 'block';
        document.getElementById("txtparcelas").style.display = 'none';
        document.getElementById("txtobservacoes").style.display = 'block';
        document.getElementById("txtbotoes").style.display = 'block';
    }
}

function MontarLayoutLancamento() {
    var tipo = document.getElementById("tipoLancamento").value;

    if (tipo == 0) {
        document.getElementById("dataLancamento").style.display = 'none';
        document.getElementById("fornecedor").style.display = 'none';
        document.getElementById("cliente").style.display = 'none';
        document.getElementById("contaContabil").style.display = 'none';
        document.getElementById("descricao").style.display = 'none';
        document.getElementById("valor").style.display = 'none';
        document.getElementById("numeroDocumento").style.display = 'none';
        document.getElementById("centroCusto").style.display = 'none';
        document.getElementById("divCategoria").style.display = 'none';
        document.getElementById("divSubCategoria").style.display = 'none';
        document.getElementById("observacoes").style.display = 'none';
        document.getElementById("footer").style.display = 'none';

        $listaCat = $("#categoria");
        $.ajax({
            url: "/Lancamentos/CarregarCategorias",
            type: "GET",
            traditional: true,
            success: function (result) {
                $listaCat.empty();
                $listaCat.append('<option value ="">-- Selecione --</option>');
                $.each(result, function (i, item) {
                    $listaCat.append('<option value = "' + item["categoriaID"] + '">' + item["nome"] + '</option>');
                });
            },
            error: function () {
                alert("Erro ao carregar Categorias");
            }
        })
    }

    if (tipo == 1) {
        document.getElementById("dataLancamento").style.display = 'block';
        document.getElementById("fornecedor").style.display = 'none';
        document.getElementById("cliente").style.display = 'block';
        document.getElementById("contaContabil").style.display = 'block';
        document.getElementById("descricao").style.display = 'block';
        document.getElementById("valor").style.display = 'block';
        document.getElementById("numeroDocumento").style.display = 'block';
        document.getElementById("centroCusto").style.display = 'block';
        document.getElementById("divCategoria").style.display = 'block';
        document.getElementById("divSubCategoria").style.display = 'block';
        document.getElementById("observacoes").style.display = 'block';
        document.getElementById("footer").style.display = 'block';

        $listaCat = $("#categoria");
        $.ajax({
            url: "/Lancamentos/CarregarCategoriasReceitas",
            type: "GET",
            traditional: true,
            success: function (result) {
                $listaCat.empty();
                $listaCat.append('<option value ="">-- Selecione --</option>');
                $.each(result, function (i, item) {
                    $listaCat.append('<option value = "' + item["categoriaID"] + '">' + item["nome"] + '</option>');
                });
            },
            error: function () {
                alert("Erro ao carregar Categorias");
            }
        })
    }

    if (tipo == 2) {
        document.getElementById("dataLancamento").style.display = 'block';
        document.getElementById("fornecedor").style.display = 'block';
        document.getElementById("cliente").style.display = 'none';
        document.getElementById("contaContabil").style.display = 'block';
        document.getElementById("descricao").style.display = 'block';
        document.getElementById("valor").style.display = 'block';
        document.getElementById("numeroDocumento").style.display = 'block';
        document.getElementById("centroCusto").style.display = 'block';
        document.getElementById("divCategoria").style.display = 'block';
        document.getElementById("divSubCategoria").style.display = 'block';
        document.getElementById("observacoes").style.display = 'block';
        document.getElementById("footer").style.display = 'block';

        $listaCat = $("#categoria");
        $.ajax({
            url: "/Lancamentos/CarregarCategoriasDespesas",
            type: "GET",
            traditional: true,
            success: function (result) {
                $listaCat.empty();
                $listaCat.append('<option value ="">-- Selecione --</option>');
                $.each(result, function (i, item) {
                    $listaCat.append('<option value = "' + item["categoriaID"] + '">' + item["nome"] + '</option>');
                });
            },
            error: function () {
                alert("Erro ao carregar Categorias");
            }
        })
    }
}

function DefinirTipoPessoa() {
    var tipo = document.getElementById("tipoPessoa").value;

    if (tipo == "PJ") {
        $("#txtCpf").prop('disabled', true);
        $("#txtCnpj").prop('disabled', false);
        $("#txtEstadual").prop('disabled', false);
        $("#txtMunicipal").prop('disabled', false);
    }

    if (tipo == "PF") {
        $("#txtCpf").prop('disabled', false);
        $("#txtCnpj").prop('disabled', true);
        $("#txtEstadual").prop('disabled', true);
        $("#txtMunicipal").prop('disabled', true);
    }

    if (tipo == "") {
        $("#txtCpf").prop('disabled', true);
        $("#txtCnpj").prop('disabled', true);
        $("#txtEstadual").prop('disabled', true);
        $("#txtMunicipal").prop('disabled', true);
    }
}

function DefinirTipoConta() {
    var tipo = document.getElementById("tipoConta").value;

    if (tipo == "") {
        $("#nomeBanco").prop('disabled', true);
        $("#numeroAgencia").prop('disabled', true);
        $("#numeroConta").prop('disabled', true);
    }

    if (tipo == "Caixa") {
        $("#nomeBanco").prop('disabled', true);
        $("#numeroAgencia").prop('disabled', true);
        $("#numeroConta").prop('disabled', true);
    }

    if (tipo == "Bancaria" || tipo == "Investimento") {
        $("#nomeBanco").prop('disabled', false);
        $("#numeroAgencia").prop('disabled', false);
        $("#numeroConta").prop('disabled', false);
    }
}

function CarregarSubCategorias() {
    $listaSub = $("#subCategoria");
    $.ajax({
        url: "/ContasPagar/CarregarSubCategorias",
        type: "GET",
        data: { id: $("#categoria").val() },
        traditional: true,
        success: function (result) {
            $listaSub.empty();
            $listaSub.append('<option value = "">-- Selecione --</option>');
            $.each(result, function (i, item) {
                $listaSub.append('<option value = "' + item["categoriaID"] + '">' + item["nome"] + '</option>');
            });
        },
        error: function () {
            alert("Erro ao carregar Sub-Categorias");
        }
    });
}


//function DefinirRecorrente() {
//    var flag = $("#isRecorrente").val();

//    if (flag == "true") {
//        $("#txtqtdParcelas").prop('disabled', true);
//        jQuery("#isParcelado").prop('disabled', 'disabled');
//        alert("ATENÇÃO! Ao definir um título como recorrente será criado 12 parcelas iguais com vencimentos recorrentes");
//    }
//    else {
//        $("#txtqtdParcelas").prop('disabled', false);
//        jQuery("#isParcelado").attr('disabled', false);
//    }
//}


//function DefinirParcelado() {
//    var flag = $("#isParcelado").val();

//    if (flag == "true") {
//        document.getElementById("txtparcelas").style.display = 'block';
//    }
//    else {
//        document.getElementById("txtparcelas").style.display = 'none';
//    }
//}


function DefinirRecorrente() {
    var flag = $("#flagRecorrente").val();

    if (flag == "true") {
        alert("ATENÇÃO! Ao definir um título como recorrente será criado 12 parcelas iguais com vencimentos recorrentes");
    }
    else {

    }
}

//Abrir modal de cadastro de novo Lançamento
$(function () {
    $(".create").click(function () {
        $("#modalCreate").load("Create", function () {
            $("#modalCreate").modal();
        })
    })
});

//Abrir modal de transferência de valores
$(function () {
    $(".transfer").click(function () {
        $("#modalTransfer").load("TransferirValores", function () {
            $("#modalTransfer").modal();
        })
    })
});

//Abrir modal de cadastro de sessão de gravação
$(function () {
    $(".createSessao").click(function () {
        $("#modalCreateSessao").load("Create", function () {
            $("#modalCreateSessao").modal();
        })
    })
});

//Abrir modal de cadastro de evento na agenda de produção
$(function () {
    $(".createAgenda").click(function () {
        $("#modalCreateAgenda").load("Create", function () {
            $("#modalCreateAgenda").modal();
        })
    })
});


//Gerar o gráfico de rosca
$(document).ready(function GerarGraficoReceitaxDespesaAcumulado() {
    $.ajax({
        url: "/Home/GerarGraficoAcumulado",
        method: "GET",
        success: function (dados) {
            $("canvas#grafico").remove();
            $("div.grafico").append('<canvas id="grafico" style="width:470px; height:250px"></canvas>');

            var ctx = document.getElementById('grafico').getContext('2d');

            var graficoDoughnut = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ['Receitas', 'Despesas'],
                    datasets: [
                        {
                            label: ['Receita', 'Despesa'],
                            data: [dados.totalReceita, dados.totalDespesa],
                            backgroundColor: ['rgba(54, 162, 235, 0.2)', 'rgba(255, 99, 132, 0.2)'],
                            borderColor: ['rgba(54, 162, 235, 1)', 'rgba(255, 99, 132, 1)'],
                            borderWidth: 1
                        },
                    ]
                },
                options: {
                    responsive: false,
                   
                }
            })
        }
    });
});

//Gerar o gráfico de barras de Receita x Despesa
$(document).ready(function GerarGraficoReceitaxDespesaMensal() {
    $.ajax({
        url: "/Home/GerarGraficoMensal",
        method: "GET",
        success: function (dados) {
            $("canvas#graficoMes").remove();
            $("div.graficoMes").append('<canvas id="graficoMes" style="width:470px; height:250px"></canvas>');

            var ctx = document.getElementById('graficoMes').getContext('2d');

            var graficoBarra = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: ['Jan', 'Fev', 'Mar', 'Abr', 'Maio', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
                    datasets: [
                        {
                            label: ['Receita'],
                            data: PegarValores(dados.somaReceitasMes),
                            backgroundColor: ['rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(54, 162, 235, 0.2)'],
                            borderColor: ['rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)', 'rgba(54, 162, 235, 1)'],
                            borderWidth: 1
                        },
                        {
                            label: ['Despesa'],
                            data: PegarValores(dados.somaDespesasMes),
                            backgroundColor: ['rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)', 'rgba(255, 99, 132, 0.2)',],
                            borderColor: ['rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)', 'rgba(255, 99, 132, 1)'],
                            borderWidth: 1
                        }
                    ]
                },
                options: {
                    responsive: false,
                    
                    scales: {
                        yAxes: [
                            {
                                ticks: {
                                    beginAtZero: true
                                }
                            }
                        ]
                    }
                }
            })
        }
    });
});

//Função auxiliar para montar os gráficos do dashboard
function PegarValores(dados) {
    var valores = [];
    var tamanho = 12;
    var indice = 0;

    for (indice; indice < tamanho; indice++) {
        valores.push(dados.value[indice])
    }

    return valores;
}

//Função para preencher o endereço quando o usuário informa o CEP.
$(document).ready(function () {
    function LimparFormularioEndereco() {
        $("#logradouro").val("");
        $("#bairro").val("");
        $("#cidade").val("");
        $("#uf").val("");
    }

    $("#cep").blur(function () {
        var cep = $(this).val().replace(/\D/g, '');

        if (cep != "") {
            var validacep = /^[0-9]{8}$/;
            if (validacep.test(cep)) {

                $("#logradouro").val("");
                $("#bairro").val("");
                $("#cidade").val("");
                $("#uf").val("");

                $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {

                    if (!("erro" in dados)) {
                        $("#logradouro").val(dados.logradouro);
                        $("#bairro").val(dados.bairro);
                        $("#cidade").val(dados.localidade);
                        $("#uf").val(dados.uf);

                    }
                    else {
                        LimparFormularioEndereco();
                        alert("CEP não encontrado.");
                    }
                });
            }
            else {
                LimparFormularioEndereco();
                alert("Formato de CEP inválido.");
            }
        }
        else {
            LimparFormularioEndereco();
        }
    });
});

function PegarNome() {
    var nomeCliente = $("#titCli option:selected").text();
    var nomeFornecedor = $("#titFor option:selected").text();

    if (nomeCliente == "-- Selecione --") {
        $("#txtNome").val(nomeFornecedor);
    }
    else {
        $("#txtNome").val(nomeCliente);
    }
}


