namespace Application.Dtos.Consulta;

public class ConsultaResponseDto
{
    public ResultData Data { get; set; }
    public bool Success { get; set; }
    public object Errors { get; set; }
    public string Message { get; set; }
}

public class ResultData
{
    public string Cnpj { get; set; }
    public string CnpjMatriz { get; set; }
    public string TipoUnidade { get; set; }
    public string RazaoSocial { get; set; }
    public string NomeFantasia { get; set; }
    public string SituacaoCadastral { get; set; }
    public string DataSituacaoCadastral { get; set; }
    public string MotivoSituacaoCadastral { get; set; }
    public string NomeCidadeExterior { get; set; }
    public string NomePais { get; set; }
    public string NaturezaJuridica { get; set; }
    public string DataInicioAtividade { get; set; }
    public string DataInicioAtividadeMatriz { get; set; }
    public string CnaePrincipal { get; set; }
    public string DescricaoTipoLogradouro { get; set; }
    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Complemento { get; set; }
    public string Bairro { get; set; }
    public string Cep { get; set; }
    public string Uf { get; set; }
    public string Municipio { get; set; }
    public string MunicipioCodigoIbge { get; set; }
    public string Telefone01 { get; set; }
    public string Telefone02 { get; set; }
    public string Fax { get; set; }
    public string CorreioEletronico { get; set; }
    public string QualificacaoResponsavel { get; set; }
    public string CapitalSocialEmpresa { get; set; }
    public string Porte { get; set; }
    public string OpcaoPeloSimples { get; set; }
    public string DataOpcaoPeloSimples { get; set; }
    public string DataExclusaoOpcaoPeloSimples { get; set; }
    public string OpcaoMei { get; set; }
    public string SituacaoEspecial { get; set; }
    public string DataSituacaoEspecial { get; set; }
    public string NomeEnteFederativo { get; set; }
    public List<Socio> Socios { get; set; }
    public List<string> CnaesSecundarios { get; set; }
}

public class Socio
{
    public string Nome { get; set; }
    public string Documento { get; set; }
    public string Qualificacao { get; set; }
}