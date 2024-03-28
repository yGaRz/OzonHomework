## �������� ������� ��������� ������ ��������

������� �� 9 �����

#~~### 1. ���������� ���������� ������ �� ������� CustomerService~~
~~* ����������� ����������� ����� `IDistributedCache` ��� `StackExchange.Redis` (�� �������� ������������� ������� ������ � StackExchange.Redis)~~

#### 2. ���������� ����������� Consumer ��� ������ pre_orders
* �������� ������ �� ������ `pre_orders`
* ��������� ������� �� ������� CustomerService
* ���� ��������� ������ ���, ��� �� ����� ��������� ��������� ������
* ����������� ������ ��������� � �����������

#### 3. ��������� ������ ����� ��������� � new_orders
* �������� ������ ������� ����� � ������������
* ���������� ���������� ����
* ��������� ���������� ����� ������� � ������ � ������� �������
* ���� ���������� ����� 5000, �� ����� �� ���������
** ��� ������������ ������ ���� ���������. (55.7522, 37.6156 � 55.01, 82.55)

* ������ ����������� ���������� �� ����������

#### 4. ���������� ����������� Poducer ��� ������ new_orders
* �������� ������ ���������� ���������� � ����� `new_orders`

#### 5. ���������� ����������� Consumer ��� ������ orders_events
* ������ ��������� �� ������ `orders_events`
* ��������� ������ ������

** �������� ��� ������ `pre_orders`
key:orderId
value:
```json
{
    "Id": 82788613,
    "Source": 1,
    "Customer": {
        "Id": 1333768,
        "Address": {
            "Region": "Montana",
            "City": "East Erich",
            "Street": "Bernier Stream",
            "Building": "0744",
            "Apartment": "447",
            "Latitude": -29.8206,
            "Longitude": -50.1263
        }
    },
    "Goods": [
        {
            "Id": 5140271,
            "Name": "Intelligent Rubber Shoes",
            "Quantity": 6,
            "Price": 2204.92,
            "Weight": 2802271506
        },
        {
            "Id": 2594594,
            "Name": "Rustic Frozen Pants",
            "Quantity": 8,
            "Price": 1576.55,
            "Weight": 3174423838
        },
        {
            "Id": 6005559,
            "Name": "Practical Plastic Soap",
            "Quantity": 2,
            "Price": 1034.51,
            "Weight": 2587375422
        }
    ]
}
```

** �������� ��� ������ `new_orders`
key:orderId
value:
```json
{"OrderId": 1}
```

** �������� ��� ������ `orders_events`
key:orderId
value:
```json
{
	"Id": 20032,
	"NewState": "SentToCustomer",
    "UpdateDate": "2023-03-11T11:40:44.964164+00:00"
}
```

������� �� 10 �����

* �������� Unit ����� ��� ����� ������