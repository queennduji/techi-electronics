## 🏗️ System Architecture

```text
                    +----------------------+
                    |        Client        |
                    |     Web / Mobile     |
                    +----------+-----------+
                               |
                               v
        +---------------------------------------------+
        |                                             |
        v                                             v
+------------------+                        +----------------------+
|     AuthAPI      |                        |     ProductAPI       |
|------------------|                        |----------------------|
| Register / Login |                        | Product Catalog      |
| JWT Auth         |                        | Admin CRUD           |
| Role Management  |                        |                      |
+--------+---------+                        +----------+-----------+
         |                                             ^
         |                                             |
         v                                             |
+------------------+        HTTP Calls         +--------+----------+
|   Message Bus    | <------------------------ | ShoppingCartAPI   |
| (Azure Service   |                           |-------------------|
|      Bus)        |                           | Cart Management   |
+--------+---------+                           | Apply Coupons     |
         |                                     | Email Requests    |
         v                                     +--------+----------+
+------------------+                                    |
|    EmailAPI      |                                    |
|------------------|                                    v
| Queue Consumer   |                          +----------------------+
| Send Emails      | -----------------------> |     CouponAPI        |
+------------------+     HTTP Calls          |----------------------|
                                             | Coupon Lookup / CRUD |
                                             +----------------------+
