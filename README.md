# �������� ������� 1
# ���������� �������������� ��������

## ���������:
~~- ������� ����������� ������� � ����� ������ ������������ ������ �������.~~
~~- �������� ������� � ������� �� ������ ����������.~~
~~- ���� ������ � ������� ������ �������.~~
~~- ������� ������� Ozon.Route256.Practice~~

## ������ �������
~~- ������ Ozon.Route256.Practice.OrdersService~~
~~- ������ Ozon.Route256.Practice.GatewayService~~
~~- ��� ����� �������� ������ ���� ������� Dockerfile~~
~~- ������� ���� docker-compose.yaml~~

## ������ docker-compose.yaml
~~- ������� �� ���� ������� �� gitlab-registry.ozon.dev
    - CustomerService
    - LogisticsSimulator
    - OrdersGenerator - � ���� ����������� � ������������ ������ ��������� WebSite, Mobile, Api
    - ServiceDiscovery~~
~~- ������� �� ���� �������� � �������
    - OrdersService - � ���� �����������
    - GatewayService~~
~~- postgress ���� ������ ��� ������� CustomerService (customer-service-db)~~
~~- postgress ���� ������ ��� ������� OrdersService (orders)~~
~~- �������������� ����� (������� � zookeeper)~~

### ���������� ������.

### �������� ���������:

������������ images:
- CustomerService
- OrdersService - � ���� �����������
- GatewayService
- LogisticsSimulator
- OrderGenerator - � ���� ����������� � ������������ ������ ��������� WebSite, Mobile, Api
- ServiceDiscovery
- kafka (broker)
- zookeeper
- postgres

��������� �������� ����������:
- CustomerService
- OrderService
- GatewayService
- LogisticsSimulator
- OrdersGenerator
- ServiceDiscovery
- kafka (broker)
- zookeeper
- postgres

p.s. �� ������� ����� �������� � ������� ���� .gitlab-ci.yml � ����������� ��������� ��� ������ �������, ������� ���� ������������ �� ��������.

�������� �� ������� ����� �������� 9 ������, �������������� ������� �� ������������� !

�������� ����� � �������� �������: 9 �����, 23:59 (�����) / 12 �����, 23:59 (��������)