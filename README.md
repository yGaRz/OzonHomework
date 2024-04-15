## �������� ������� ������ ������ ��������

#### ������� �� 9 �����

- ������� � ��������� 2 �������� �� ��� `order-service`
- ���������������� `service-discovery` ��� ����� ��������� (`orders-1, buckets 0-3; orders-2, buckets 4-7`)
- ����������� ������� ��� ����������� ������ �� �����
- ��������� `connectionFactory` �� ������ � ������������� ����� ����� `service-discovery`
- ���������� �������� �� ������ � ������������� �����
- ��������� �������� �� ������������� ������� (�� �������)
- ��������� ������ ������� �� ������������� ����

#### ������� �� 10 ������

- ����������� ��������� ����� ��� ��������� ������� �� �� ����� ������������ (�������� ����������)
- ���������������� �������� �� ��������� ������, �� ������� ���������� ���������� ������, ������������ �� ��������� (������� �� ����� �������)

### FAQ

##### ��� ���� � �������, ������� ���� �� �� ����� � ���������� � �.�.?

��� ���������� ������� ����������, ����� �� ���������� ��� ������ �� ���� �������. �� ��� ����������, �������������� ����� �� �� ����� ������������, ����� ����������� ���������� ������ (*������� �� 10 ������*). ��������� -  � ������ ����� �������� ����������� ���� ��� ����������

##### ��� ���� � ������� ���������� � ����� �� � ������/�������?

��� ��� ��� �����������, �� �� ������ ��������� ������ ���������� �� ��� ����� � ������� � ������ ����� �����. �� ������ ����������� ����� � ��, �� �� �������� ����� `foreign key constraint`, ���� ��� � ��� ����.

##### ��� ���� � �����������������?

��� ���������� ������� � ��������� ��� ���������� ����������� ������� �� ���������, ����� ��� ���������� ���� ��������� ������������ � ��������/���������� �������� ��������, �.�. � ����� ������ ��� ����� ����������� �� ������ ����� � ��������� �������������� ����������. ��� ����, ������� ��������� ���������������� ���������� ����� ��������� � ������ ������ �������

##### ��� ���� � ������������� ��������?

���������� ������� ������� �������� �������� �� ��� �����. ��� ������������� ����������� `unnest` ����������� ��������� ���������������� ��� � ���� ������ � ����� `public` (����� ��������)

#### �������� �������:

1. ������������ ���� ������ `orders-1` � `orders-2`
2. � ���� `orders-1` ��������� ����� `bucket_0, bucket_1, bucket_2, bucket_3`
3. � ���� orders-2 ��������� ����� `bucket_4, bucket_5, bucket_6, bucket_7`
4. � ������ ����� `bucket_N` ������������ ��������� ��
5. ���������� `IShardingRule`, ����������� �������������� ����� ������������ � ����� ������
6. ���������� `ShardConnectionFactory`, ��������� ����������� � ������� �������� �� �� ������ �����
7. ����������� ��������� �� ������������� `ShardConnectionFactory`
8. ��� API �������� ��� � ������.

#### ��� ������� �� 10 ������:

1. ������� ��������� ������� ��� ������ ������� �� �� ����� ������������. ���������� �������� ������������ ����������� �� ����� �������, �� ��������� ����������� ���� �� ����
2. ��� �������/����������, ������������� ��������� ����/�������� ������, ����� ����������� ��������� �������/� (��� ����������)
3. ����� ������� �� �� ����� ������������ ������������ ����� ��������� �������/�, � �� �� ���� �������

#### �������: 
13 ������, 23:59 (�����) / 16 ������, 23:59 (��������)
