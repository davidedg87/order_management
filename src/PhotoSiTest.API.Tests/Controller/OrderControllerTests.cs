using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PhotoSi.AddressService.Core.Dtos;
using PhotoSi.AddressService.Core.Interfaces;
using PhotoSi.OrderService.Core.Dtos;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSi.ProductService.Core.Interfaces;
using PhotoSi.UserService.Core.Dtos;
using PhotoSi.UserService.Core.Interfaces;
using PhotoSiTest.API.Controllers;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.ProductService.Core.Dtos;

namespace PhotoSiTest.API.Tests.Controller
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IAddressService> _addressServiceMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _userServiceMock = new Mock<IUserService>();
            _addressServiceMock = new Mock<IAddressService>();
            _productServiceMock = new Mock<IProductService>();

            _controller = new OrderController(_orderServiceMock.Object, _userServiceMock.Object, _addressServiceMock.Object, _productServiceMock.Object);
        }

        #region GetOrderById

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            _orderServiceMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync((OrderDto)null);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetOrderById_ReturnsOk_WithOrderDetails_WhenOrderExists()
        {
            // Arrange
            var orderId = 1;
            var order = new OrderDto { Id = orderId, UserId = 1, AddressId = 1, ProductIds = new List<int> { 1 } };
            var user = new UserDto { Id = 1, Name = "John Doe" };
            var address = new AddressDto { Id = 1, FullAddress = "123 Main St" };
            var productCodes = new List<ProductCodeDto> { new ProductCodeDto { ProductId = 1, Code = "ABC123" } };

            _orderServiceMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
            _userServiceMock.Setup(x => x.GetByIdAsync(order.UserId)).ReturnsAsync(user);
            _addressServiceMock.Setup(x => x.GetByIdAsync(order.AddressId)).ReturnsAsync(address);
            _productServiceMock.Setup(x => x.GetProductCodesByIdsAsync(order.ProductIds)).ReturnsAsync(productCodes);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var returnedOrder = okResult.Value.Should().BeAssignableTo<OrderDto>().Subject;

            returnedOrder.UserName.Should().Be("John Doe");
            returnedOrder.AddressFull.Should().Be("123 Main St");
            returnedOrder.ProductCodes.Should().Contain("ABC123");
        }

        #endregion

        #region GetAllOrders

        [Fact]
        public async Task GetAllOrders_ReturnsEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            _orderServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<OrderDto>());

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var orders = okResult.Value.Should().BeAssignableTo<List<OrderDto>>().Subject;
            orders.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllOrders_ReturnsOrders_WithDetails_WhenOrdersExist()
        {
            // Arrange
            var orders = new List<OrderDto>
        {
            new OrderDto { Id = 1, UserId = 1, AddressId = 1, ProductIds = new List<int> { 1 } }
        };
            var user = new UserDto { Id = 1, Name = "John Doe" };
            var address = new AddressDto { Id = 1, FullAddress = "123 Main St" };
            var productCodes = new List<ProductCodeDto> { new ProductCodeDto { ProductId = 1, Code = "ABC123" } };

            _orderServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(orders);
            _userServiceMock.Setup(x => x.GetByIdAsync(orders[0].UserId)).ReturnsAsync(user);
            _addressServiceMock.Setup(x => x.GetByIdAsync(orders[0].AddressId)).ReturnsAsync(address);
            _productServiceMock.Setup(x => x.GetProductCodesByIdsAsync(orders[0].ProductIds)).ReturnsAsync(productCodes);

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var returnedOrders = okResult.Value.Should().BeAssignableTo<List<OrderDto>>().Subject;
            var returnedOrder = returnedOrders.First();

            returnedOrder.Id.Should().Be(1);
            returnedOrder.UserId.Should().Be(1);
            returnedOrder.AddressId.Should().Be(1);
            returnedOrder.ProductIds.Should().Contain(1);
        }


        [Fact]
        public async Task GetAllOrders_ShouldAssignAddressToOrder_WhenAddressExists()
        {
            // Arrange
            var orders = new List<OrderDto>
            {
                new OrderDto { Id = 1, AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } }
            };


            var address = new AddressDto { Id = 1, FullAddress = "123 Test St" };
            var addresses = new List<AddressDto> { address };
            var productCodes = new List<ProductCodeDto> { new ProductCodeDto { ProductId = 1, Code = "ABC123" } };


            _orderServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(orders);
            _addressServiceMock.Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(addresses);
            _productServiceMock.Setup(x => x.GetProductCodesByIdsAsync(orders[0].ProductIds)).ReturnsAsync(productCodes);

            // Act
            var result = await _controller.GetAllOrders();
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var resultOrders = okResult.Value.Should().BeAssignableTo<List<OrderDto>>().Which;

            // Assert
            resultOrders.Should().HaveCount(1);  // Verifica che ci sia almeno un ordine
            resultOrders.First().AddressFull.Should().Be(address.FullAddress);  // Verifica che l'indirizzo sia stato associato correttamente all'ordine
        }

        [Fact]
        public async Task GetAllOrders_ShouldAssignUserNameToOrder_WhenUserExists()
        {
            var dateTime = DateTime.UtcNow;
            // Arrange
            var orders = new List<OrderDto>
            {
                new OrderDto { Id = 1, AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } , OrderDate= dateTime, TotalAmount = 200, Status = OrderService.Core.Models.OrderStatus.Pending }
            };

            var user = new UserDto { Id = 1, Name = "John Doe" };
            var users = new List<UserDto> { user };
            var productCodes = new List<ProductCodeDto> { new ProductCodeDto { ProductId = 1, Code = "ABC123" } };

            _orderServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(orders);
            _userServiceMock.Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(users);
            _productServiceMock.Setup(x => x.GetProductCodesByIdsAsync(orders[0].ProductIds)).ReturnsAsync(productCodes);

            // Act
            var result = await _controller.GetAllOrders();
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var resultOrders = okResult.Value.Should().BeAssignableTo<List<OrderDto>>().Which;

            // Assert
            resultOrders.Should().HaveCount(1);  // Verifica che ci sia almeno un ordine
            resultOrders.First().UserName.Should().Be(user.Name);  // Verifica che il nome utente sia stato associato correttamente all'ordine
        }

        #endregion

        #region Paginate Orders

        [Fact]
        public async Task GetPaginatedOrders_ShouldReturnPaginatedOrders_WhenValidFilterIsPassed()
        {
            // Arrange: Setup del filtro di paginazione
            var filter = new PagedFilter { PageNumber = 1, PageSize = 10 };

            // Setup del risultato della paginazione
            var ordersResult = new PaginatedResult<OrderDto>
            {
                Items = new List<OrderDto>
            {
                new OrderDto { Id = 1, AddressId = 1, ProductIds = new List<int> { 1 }, UserId = 1 },
                new OrderDto { Id = 2, AddressId = 2, ProductIds = new List<int> { 2 }, UserId = 2 }
            },
                TotalCount = 20,
                PageNumber = 1,
                PageSize = 10
            };

            // Mock dei metodi di servizio
            _orderServiceMock.Setup(service => service.PaginateAsync(filter))
                             .ReturnsAsync(ordersResult);

            // Mock dei metodi per recuperare indirizzi, prodotti e utenti
            _addressServiceMock.Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>()))
                               .ReturnsAsync(new List<AddressDto>
                               {
                               new AddressDto { Id = 1, FullAddress = "Address 1" },
                               new AddressDto { Id = 2, FullAddress = "Address 2" }
                               });
            _productServiceMock.Setup(service => service.GetProductCodesByIdsAsync(It.IsAny<List<int>>()))
                               .ReturnsAsync(new List<ProductCodeDto>
                               {
                               new ProductCodeDto { ProductId = 1, Code = "P001" },
                               new ProductCodeDto { ProductId = 2, Code = "P002" }
                               });
            _userServiceMock.Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>()))
                            .ReturnsAsync(new List<UserDto>
                            {
                            new UserDto { Id = 1, Name = "User 1" },
                            new UserDto { Id = 2, Name = "User 2" }
                            });

            // Act: Esegui la chiamata al controller
            var result = await _controller.GetPaginatedOrders(filter);

            // Assert: Verifica il risultato
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var returnValue = okResult.Value.Should().BeOfType<PaginatedResult<OrderDto>>().Which;

            // Verifica il numero di ordini restituiti
            returnValue.Items.Should().HaveCount(2);
            returnValue.TotalCount.Should().Be(20);
            returnValue.PageNumber.Should().Be(1);
            returnValue.PageSize.Should().Be(10);

            // Verifica che gli ordini abbiano gli indirizzi, i codici dei prodotti e i nomi utente associati
            returnValue.Items.Should().Contain(order => order.AddressFull == "Address 1" && order.ProductCodes.Contains("P001") && order.UserName == "User 1");
            returnValue.Items.Should().Contain(order => order.AddressFull == "Address 2" && order.ProductCodes.Contains("P002") && order.UserName == "User 2");
        }

        [Fact]
        public async Task GetPaginatedOrders_ShouldReturnBadRequest_WhenPageNumberIsZeroOrNegative()
        {
            // Arrange: Setup di un filtro con PageNumber <= 0
            var filter = new PagedFilter { PageNumber = 0, PageSize = 10 };

            // Act: Esegui la chiamata al controller
            var result = await _controller.GetPaginatedOrders(filter);

            // Assert: Verifica che venga restituito un BadRequest
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("PageNumber must be greater than zero.");
        }

        [Fact]
        public async Task GetPaginatedOrders_ShouldReturnBadRequest_WhenPageSizeIsZeroOrNegative()
        {
            // Arrange: Setup di un filtro con PageSize <= 0
            var filter = new PagedFilter { PageNumber = 1, PageSize = 0 };

            // Act: Esegui la chiamata al controller
            var result = await _controller.GetPaginatedOrders(filter);

            // Assert: Verifica che venga restituito un BadRequest
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be("PageSize must be greater than zero.");
        }

        [Fact]
        public async Task GetPaginatedOrders_ShouldReturnEmptyList_WhenNoOrdersExist()
        {
            // Arrange: Setup del filtro di paginazione
            var filter = new PagedFilter { PageNumber = 1, PageSize = 10 };

            // Setup: La lista degli ordini è vuota
            var ordersResult = new PaginatedResult<OrderDto>
            {
                Items = new List<OrderDto>(),  // Nessun ordine
                TotalCount = 0,  // Contatore totale pari a 0
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            // Mock del servizio per restituire una lista vuota di ordini
            _orderServiceMock.Setup(service => service.PaginateAsync(filter))
                             .ReturnsAsync(ordersResult);

            // Act: Esegui la chiamata al controller
            var result = await _controller.GetPaginatedOrders(filter);

            // Assert: Verifica che venga restituito un OkObjectResult
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            var returnValue = okResult.Value.Should().BeOfType<PaginatedResult<OrderDto>>().Which;

            // Verifica che la lista degli ordini sia vuota e il contatore totale sia 0
            returnValue.Items.Should().BeEmpty();
            returnValue.TotalCount.Should().Be(0);
            returnValue.PageNumber.Should().Be(filter.PageNumber);
            returnValue.PageSize.Should().Be(filter.PageSize);
        }

        #endregion

        #region CreateOrder

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenAddressDoesNotExist()
        {
            var dateTime = DateTime.UtcNow;
            // Arrange
            var orderDto = new OrderEditDto { AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 }, OrderDate = dateTime, Status = OrderService.Core.Models.OrderStatus.Pending };
            _addressServiceMock.Setup(x => x.GetByIdAsync(orderDto.AddressId)).ReturnsAsync((AddressDto)null);

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.Value.Should().Be("Address with ID 1 does not exist.");
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var orderDto = new OrderEditDto { AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } };
            var address = new AddressDto { Id = 1, FullAddress = "123 Main St" };
            _addressServiceMock.Setup(x => x.GetByIdAsync(orderDto.AddressId)).ReturnsAsync(address);
            _userServiceMock.Setup(x => x.GetByIdAsync(orderDto.UserId)).ReturnsAsync((UserDto)null);

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.Value.Should().Be("User with ID 1 does not exist.");
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenSomeProductsDoNotExist()
        {
            // Arrange
            var orderDto = new OrderEditDto { AddressId = 1, UserId = 1, ProductIds = new List<int> { 1, 2 } };
            var address = new AddressDto { Id = 1, FullAddress = "123 Main St" };
            var user = new UserDto { Id = 1, Name = "John Doe" };
            _addressServiceMock.Setup(x => x.GetByIdAsync(orderDto.AddressId)).ReturnsAsync(address);
            _userServiceMock.Setup(x => x.GetByIdAsync(orderDto.UserId)).ReturnsAsync(user);
            _productServiceMock.Setup(x => x.GetByIdsAsync(orderDto.ProductIds)).ReturnsAsync(new List<ProductDto> { new ProductDto { Id = 1 } });

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.Value.Should().Be("Some of the products do not exist.");
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedAtAction_WhenOrderIsValid()
        {
            // Arrange
            var orderDto = new OrderEditDto { AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } };
            var address = new AddressDto { Id = 1, FullAddress = "123 Main St" };
            var user = new UserDto { Id = 1, Name = "John Doe" };
            var product = new ProductDto { Id = 1, Name = "Test Product", Price = 10 };
            _addressServiceMock.Setup(x => x.GetByIdAsync(orderDto.AddressId)).ReturnsAsync(address);
            _userServiceMock.Setup(x => x.GetByIdAsync(orderDto.UserId)).ReturnsAsync(user);
            _productServiceMock.Setup(x => x.GetByIdsAsync(orderDto.ProductIds)).ReturnsAsync(new List<ProductDto> { product });
            _productServiceMock.Setup(x => x.GetSumAmount(orderDto.ProductIds)).ReturnsAsync(10);

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Which;
            createdResult.ActionName.Should().Be("GetOrderById");
            createdResult.RouteValues["id"].Should().Be(0);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenInvalidModel()
        {
            // Arrange
            var orderDto = new OrderEditDto { AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } };

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnBadRequest_WhenDuplicateOrderExists()
        {
            // Arrange
            var orderDto = new OrderEditDto
            {
                Id = 1,
                UserId = 1,
                AddressId = 1,
                ProductIds = new List<int> { 1, 2 },
                TotalAmount = 100
            };

            _orderServiceMock.Setup(service => service.IsDuplicateOrderAsync(orderDto))
                .ReturnsAsync(true); // Simula che l'ordine sia duplicato

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("An order with the same values already exists.");
        }


        #endregion

        #region UpdateOrder

        [Fact]
        public async Task UpdateOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            var orderDto = new OrderEditDto { Id = 1, AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } };
            _orderServiceMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync((OrderDto)null);

            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateOrder_ReturnsBadRequest_WhenAddressDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            var orderDto = new OrderEditDto { Id =1, AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } };
            var order = new OrderDto { Id = orderId, UserId = 1, AddressId = 1, ProductIds = new List<int> { 1 } };
            var user = new UserDto { Id = 1, Name = "John Doe" };
            _orderServiceMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
            _userServiceMock.Setup(x => x.GetByIdAsync(orderDto.UserId)).ReturnsAsync(user);
            _addressServiceMock.Setup(x => x.GetByIdAsync(orderDto.AddressId)).ReturnsAsync((AddressDto)null);

            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.Value.Should().Be("Address with ID 1 does not exist.");
        }

        [Fact]
        public async Task UpdateOrder_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            var orderDto = new OrderEditDto { Id = 1, AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } };
            var order = new OrderDto { Id = orderId, UserId = 1, AddressId = 1, ProductIds = new List<int> { 1 } };
            var address = new AddressDto { Id = 1, FullAddress = "123 Main St" };
            _orderServiceMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
            _addressServiceMock.Setup(x => x.GetByIdAsync(orderDto.AddressId)).ReturnsAsync(address);
            _userServiceMock.Setup(x => x.GetByIdAsync(orderDto.UserId)).ReturnsAsync((UserDto)null);

            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.Value.Should().Be("User with ID 1 does not exist.");
        }

        [Fact]
        public async Task UpdateOrder_ReturnsOk_WhenOrderIsUpdatedSuccessfully()
        {
            // Arrange
            var orderId = 1;
            var orderDto = new OrderEditDto { Id = 1, AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } };
            var order = new OrderDto { Id = orderId, UserId = 1, AddressId = 1, ProductIds = new List<int> { 1 } };
            var address = new AddressDto { Id = 1, FullAddress = "123 Main St" };
            var user = new UserDto { Id = 1, Name = "John Doe" };
            var product = new ProductDto { Id = 1, Name = "Test Product", Price = 10 };
            _orderServiceMock.Setup(x => x.GetByIdAsync(orderId)).ReturnsAsync(order);
            _addressServiceMock.Setup(x => x.GetByIdAsync(orderDto.AddressId)).ReturnsAsync(address);
            _userServiceMock.Setup(x => x.GetByIdAsync(orderDto.UserId)).ReturnsAsync(user);
            _productServiceMock.Setup(x => x.GetByIdsAsync(orderDto.ProductIds)).ReturnsAsync(new List<ProductDto> { product });

            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateOrder_ReturnsBadRequest_WhenInvalidModel()
        {
            // Arrange
            var orderId = 1;
            var orderDto = new OrderEditDto { Id = 1, AddressId = 1, UserId = 1, ProductIds = new List<int> { 1 } };

            // Simula che il modello non sia valido
            _controller.ModelState.AddModelError("Model", "Invalid model");


            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<SerializableError>()
                .Which.Should().ContainKey("Model");
        }

        [Fact]
        public async Task UpdateOrder_ShouldReturnBadRequest_WhenSomeProductsDoNotExist()
        {
            // Arrange
            var orderId = 1;
            var orderDto = new OrderEditDto
            {
                Id = orderId,
                UserId = 1,
                AddressId = 1,
                ProductIds = new List<int> { 1, 2, 999 }  // Include un prodotto che non esiste (999)
            };

            // Mock del servizio che restituisce solo alcuni dei prodotti esistenti
            _productServiceMock.Setup(service => service.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<ProductDto>  // Solo i prodotti con ID 1 e 2 esistono
                {
            new ProductDto { Id = 1 },
            new ProductDto { Id = 2 }
                });

            // Mock del servizio che restituisce l'ordine esistente
            _orderServiceMock.Setup(service => service.GetByIdAsync(orderId))
                .ReturnsAsync(new OrderDto { Id = orderId });

            // Mock del servizio utente e indirizzo
            _userServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new UserDto { Id = 1, Name = "User" });

            _addressServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new AddressDto { Id = 1, FullAddress = "123 Address St" });

            // Act: Chiamata al controller per aggiornare l'ordine
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert: Verifica che il risultato sia un BadRequest
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.StatusCode.Should().Be(400);  // Verifica il codice di stato HTTP
            badRequestResult.Value.Should().Be("Some of the products do not exist.");  // Verifica il messaggio di errore
        }


        [Fact]
        public async Task UpdateOrder_ShouldReturnBadRequest_WhenIdInRouteDoesNotMatchIdInBody()
        {
            // Arrange
            var routeId = 1;  // ID passato nell'URL della richiesta
            var orderDto = new OrderEditDto
            {
                Id = 2,  // ID nel corpo della richiesta (differente da quello nell'URL)
                UserId = 1,
                AddressId = 1,
                ProductIds = new List<int> { 1, 2 }
            };

            // Mock del servizio che restituisce un ordine esistente
            _orderServiceMock.Setup(service => service.GetByIdAsync(routeId))
                .ReturnsAsync(new OrderDto { Id = routeId });

            // Act: Chiamata al controller per aggiornare l'ordine
            var result = await _controller.UpdateOrder(routeId, orderDto);

            // Assert: Verifica che il risultato sia un BadRequest
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Which;
            badRequestResult.StatusCode.Should().Be(400);  // Verifica che il codice di stato HTTP sia 400
            badRequestResult.Value.Should().Be("ID mismatch between route and body.");  // Verifica il messaggio di errore
        }

        #endregion

        #region DeleteOrder

        [Fact]
        public async Task DeleteOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            _orderServiceMock.Setup(service => service.GetByIdAsync(orderId)).ReturnsAsync((OrderDto)null);

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Order with ID {orderId} not found.");
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNoContent_WhenOrderDeletedSuccessfully()
        {
            // Arrange
            int orderId = 1;
            _orderServiceMock.Setup(s => s.GetByIdAsync(orderId)).ReturnsAsync(new OrderDto { Id = orderId });

            // Act
            var result = await _controller.DeleteOrder(orderId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _orderServiceMock.Verify(s => s.DeleteAsync(orderId), Times.Once);
        }

        #endregion
    }

}
