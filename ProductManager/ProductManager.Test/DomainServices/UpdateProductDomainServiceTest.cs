﻿using AutoMapper;
using FluentValidation;
using Moq;
using ProductManager.Application.AppServices;
using ProductManager.Application.Helpers;
using ProductManager.Application.Models.DTO;
using ProductManager.Domain.Contracts.DomainServices;
using ProductManager.Domain.Contracts.Repository;
using ProductManager.Domain.DomainServices;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Validators.ProductValidator;
using ProductManager.Test.Builders;
using System;
using Xunit;

namespace ProductManager.Test.DomainServices
{
    public class UpdateProductDomainServiceTest
    {
        private readonly CreateProductValidator _createprodutoValidator;
        private readonly UpdateProductValidator _updateprodutoValidator;
        private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
        private readonly IProdutoDomainService _produtoDomainService;
        private readonly IMapper _mapper;

        public UpdateProductDomainServiceTest()
        {
            _createprodutoValidator = new CreateProductValidator();
            _updateprodutoValidator = new UpdateProductValidator();
            _produtoRepositoryMock = new Mock<IProdutoRepository>();
            _produtoDomainService = new ProdutoDomainService(
                _produtoRepositoryMock.Object,
                _createprodutoValidator,
                _updateprodutoValidator);

            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new AutoMapperProfile());
                });
                IMapper mapper = mappingConfig.CreateMapper();
                _mapper = mapper;
            }
        }

        [Fact]
        public async void UpdateProduto_Success()
        {
            //Arrange
            var domainServiceMoq = new Mock<IProdutoDomainService>();
            domainServiceMoq.Setup(x => x.UpdateAsync(It.IsAny<Produto>()));

            var produtoAppService = new ProdutoAppService(domainServiceMoq.Object, _mapper);

            var produtoDto = new ProdutoDto { Id = Guid.NewGuid(), Nome = "Test product" };

            //Act
            await produtoAppService.UpdateAsync(produtoDto);

            //Assert
            domainServiceMoq.Verify(x => x.UpdateAsync(It.IsAny<Produto>()));
        }

        [Fact]
        public async void Update_WithIdEmptyShouldThrowValidationException()
        {
            //Arrange
            var entity = new ProductBuilder().WithIdEmpty();

            //Act/Assert
            await Assert.ThrowsAsync<ValidationException>(
                async () => await _produtoDomainService.UpdateAsync(entity));
        }
    }
}
