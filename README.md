# Sipariş Yönetim Sistemi API

E-ticaret platformu için geliştirilmiş basit bir sipariş yönetim sistemi. ASP.NET Core 8.0 Web API kullanılarak oluşturulmuştur.

## 🚀 Özellikler

- ✅ Yeni sipariş oluşturma
- ✅ Siparişleri listeleme
- ✅ Sipariş detaylarını görüntüleme
- ✅ Sipariş silme
- ✅ Otomatik stok kontrolü
- ✅ Ürün yönetimi
- ✅ Transaction yönetimi
- ✅ Swagger/OpenAPI dokümantasyonu

## 📋 Gereksinimler

- .NET 8.0 SDK
- Visual Studio 2022, VS Code veya Rider

## 🛠️ Kurulum ve Çalıştırma

### 1. Projeyi Klonlayın

```bash
git clone <repository-url>
cd "Temel Siparis Yönetim Sistemi"
```

### 2. Bağımlılıkları Yükleyin

```bash
dotnet restore
```

### 3. Projeyi Çalıştırın

```bash
dotnet run
```

### 4. Tarayıcınızda Açın

Uygulama varsayılan olarak şu adreslerde çalışacaktır:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

Swagger UI otomatik olarak ana sayfada açılacaktır: **http://localhost:5000**

## 📚 API Endpoints

### Siparişler (Orders)

#### 1. Yeni Sipariş Oluştur
```http
POST /api/orders
Content-Type: application/json

{
  "customerName": "Ahmet Yılmaz",
  "customerEmail": "ahmet@example.com",
  "shippingAddress": "İstanbul, Türkiye",
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 2,
      "quantity": 1
    }
  ]
}
```

**Başarılı Yanıt (201 Created):**
```json
{
  "id": 1,
  "customerName": "Ahmet Yılmaz",
  "customerEmail": "ahmet@example.com",
  "shippingAddress": "İstanbul, Türkiye",
  "totalAmount": 2629.97,
  "status": "Pending",
  "orderDate": "2024-01-15T10:30:00Z",
  "shippedDate": null,
  "deliveredDate": null,
  "orderItems": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Laptop",
      "quantity": 2,
      "unitPrice": 1299.99,
      "totalPrice": 2599.98
    },
    {
      "id": 2,
      "productId": 2,
      "productName": "Wireless Mouse",
      "quantity": 1,
      "unitPrice": 29.99,
      "totalPrice": 29.99
    }
  ]
}
```

**Hata Durumları:**
- `400 Bad Request`: Geçersiz veri veya yetersiz stok
- `500 Internal Server Error`: Sunucu hatası

#### 2. Tüm Siparişleri Listele
```http
GET /api/orders
```

**Başarılı Yanıt (200 OK):**
```json
[
  {
    "id": 1,
    "customerName": "Ahmet Yılmaz",
    "customerEmail": "ahmet@example.com",
    "shippingAddress": "İstanbul, Türkiye",
    "totalAmount": 2629.97,
    "status": "Pending",
    "orderDate": "2024-01-15T10:30:00Z",
    "orderItems": [...]
  }
]
```

#### 3. Sipariş Detayını Getir
```http
GET /api/orders/{id}
```

**Başarılı Yanıt (200 OK):**
```json
{
  "id": 1,
  "customerName": "Ahmet Yılmaz",
  "customerEmail": "ahmet@example.com",
  "shippingAddress": "İstanbul, Türkiye",
  "totalAmount": 2629.97,
  "status": "Pending",
  "orderDate": "2024-01-15T10:30:00Z",
  "orderItems": [...]
}
```

**Hata Durumları:**
- `404 Not Found`: Sipariş bulunamadı

#### 4. Sipariş Sil
```http
DELETE /api/orders/{id}
```

**Başarılı Yanıt (204 No Content)**

**Hata Durumları:**
- `404 Not Found`: Sipariş bulunamadı

### Ürünler (Products)

#### 1. Tüm Ürünleri Listele
```http
GET /api/products
```

**Başarılı Yanıt (200 OK):**
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "description": "High-performance laptop for professionals",
    "price": 1299.99,
    "stockQuantity": 50,
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

#### 2. Ürün Detayını Getir
```http
GET /api/products/{id}
```

**Başarılı Yanıt (200 OK):**
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "High-performance laptop for professionals",
  "price": 1299.99,
  "stockQuantity": 50,
  "createdAt": "2024-01-01T00:00:00Z",
  "updatedAt": "2024-01-01T00:00:00Z"
}
```

## 🗂️ Proje Yapısı

```
OrderManagementSystem/
├── Controllers/
│   ├── OrdersController.cs      # Sipariş endpoint'leri
│   └── ProductsController.cs    # Ürün endpoint'leri
├── Data/
│   └── ApplicationDbContext.cs  # Entity Framework DbContext
├── DTOs/
│   ├── CreateOrderDto.cs        # Sipariş oluşturma DTO
│   └── OrderResponseDto.cs      # Sipariş yanıt DTO
├── Models/
│   ├── Order.cs                 # Sipariş modeli
│   ├── OrderItem.cs             # Sipariş kalemi modeli
│   └── Product.cs               # Ürün modeli
├── Services/
│   ├── IOrderService.cs         # Sipariş servis interface
│   └── OrderService.cs          # Sipariş servis implementasyonu
├── Properties/
│   └── launchSettings.json      # Launch ayarları
├── appsettings.json             # Uygulama ayarları
├── Program.cs                   # Uygulama giriş noktası
└── OrderManagementSystem.csproj # Proje dosyası
```

## 💾 Veritabanı

Proje **In-Memory Database** kullanmaktadır:
- Harici bir veritabanı kurulumuna gerek yoktur
- Uygulama her başlatıldığında örnek verilerle başlar
- Basit ve hızlı test için idealdir

### Örnek Ürünler

Uygulama başlatıldığında aşağıdaki ürünler otomatik olarak yüklenir:

1. **Laptop** - 1299.99 TL (Stok: 50)
2. **Wireless Mouse** - 29.99 TL (Stok: 200)
3. **Mechanical Keyboard** - 89.99 TL (Stok: 100)
4. **USB-C Hub** - 49.99 TL (Stok: 75)
5. **Webcam HD** - 79.99 TL (Stok: 30)

## 🔒 İş Kuralları

### Sipariş Oluşturma
- Müşteri adı, e-posta ve teslimat adresi zorunludur
- En az bir ürün içermelidir
- Stok kontrolü otomatik yapılır
- Yetersiz stok durumunda hata döner
- Başarılı siparişte stok otomatik azaltılır
- Transaction ile veri tutarlılığı sağlanır

### Sipariş Silme
- Sipariş silindiğinde stok miktarı geri yüklenir
- Cascade delete ile sipariş kalemleri otomatik silinir


## 🔧 Teknolojiler

- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 8.0** - ORM
- **In-Memory Database** - Veritabanı
- **Swagger/OpenAPI** - API dokümantasyonu

## 📝 Notlar

- Bu basit bir sipariş yönetim sistemidir
- In-Memory database kullanıldığı için uygulama her başlatıldığında veriler sıfırlanır
- Production için SQL Server veya PostgreSQL kullanılması önerilir
- Geliştirme ve test amaçlıdır

## 📄 Lisans

Bu proje MIT lisansı altında Krila Consultancy tarafından geliştirilmiştir.

