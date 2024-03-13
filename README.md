�������� ������� 2

~~1. ���������� ����������� �������������� � SD � ������� OrderService.~~
~~������ �� SD ����� ���� �� 6 ������, ����� ������ ��������� ������������� ��.~~

2. ���������� ����������� ����� GRPC ������� � OrdersService (�������� ������� �� �������)
~~����� ������ ������~~
~~����� �������� ������� ������~~
~~����� �������� ������ ��������~~
~~����� �������� ������ �������~~
~~����� ��������� ������� �� �������~~
~~����� ��������� ���� ������� �������~~

GRPC API

~~GRPC ����� ������ ��������� ������ ��������� ��� ��������������. ���������� ���� ����� �� ���������.~~
~~��� �����, ������� ����� ������� ������ ������ - ������ ���������� ������ ������.~~
~~���� ����� ����� ������� ��� NotFound - ������ ��� ������ ���������� ������ ���� ���.~~
����������� �������� ��������� ��������� �� �������� ��������� � ���������� ��� 400 ��� REST �������� 
��� StatusCode.InvalidArgument, ���� ��������� �� ������ ���������.


3. ���������� ����������� ����� ��� REST API � ������� Gateway - ��� ����� � Gateway 
���������� ������ � ��������������� ������ � grpc ������� OrderService � CustomerService

����� ������ ������
����� �������� ������� ������
����� �������� ������ ��������
����� �������� ������ ��������
����� �������� ������ �������
����� ��������� ������� �� �������
����� ��������� ���� ������� �������


REST API:

����� ������ ������������ � swagger.
���������� ������������ � ������ �� ���������� ������������ ���� ������ �� ������ �������, ������� ��� ��������
������ �������� ��������������� ������ � �������� OrderService � CustomerService, �.�. ������������� REST ������ � GRPC ������, 
������� �����, �������� GRPC ����� ��� ������ � ������������� ��� � REST �����.
����� ������������� �������� - ������� ������ ������ � ��� ���� �� ������.

~~4. ������ ��� OrderService ������ ����� ������������� �������� �� ������ ���������� ����� ������� (������ ��� ��� ������� ��� � ��������).~~
~~replicas � docker-compose ������������ �� �����. ��� ������� ������ ���� ��������� �������.~~

~~�� ����������� ������������ ��������� ����������, ����� ��������� ���������� � ���, ��� ���� ������������� �� workshop.~~
https://hallowed-join-b37.notion.site/9a297c7d74ae4b8a93feae6d8af36531

5. �������������� ������� �� 10 ������

� �������� ��������� ��� ���������� �� �������, �������� ��������� ������ � ������ ����� 200, 400, 500
��� ���������� �������� �������� UnitTest, �������� ��� ������� ���������� � SD, ��� ��� �������, ������� ��������� �������������� �������� (���������). ��� ����� ������ ����� �� ������ ����� �� ���������.
��� UnitTest ������������ xUnit


6. ��������� ������������ ��������:

��� ������ ������:
������ ������ ������ - ������� �������� ��� ���
�������� ������ �� ��������� ������, ���� �������� ����� �� �������. ���� ������ ������� - ���� ������.


��� ����� ���������:
�������� �������
���������� �������
����� ����� �������
��������� ���
���������� ��������, ��������� ����� � ���� �������

�������� ����� � �������� �������: 16 �����, 23:59 (�����) / 19 �����, 23:59 (��������)