                    +------------------+
                    |      Client      |
                    |  Web / Mobile    |
                    +---------+--------+
                              |
                              v
        +---------------------+----------------------+
        |                                            |
        v                                            v
+---------------+                           +-------------------+
|    AuthAPI    |                           |   ProductAPI      |
| register/login|                           | product catalog   |
| assign roles  |                           | admin CRUD        |
+-------+-------+                           +---------+---------+
        |                                             ^
        |                                             |
        |                                             |
        v                                             |
+---------------+         HTTP calls          +-------+--------+
|  MessageBus   | <-------------------------  | ShoppingCartAPI |
| Azure Service |                             | cart orchestration
| Bus publisher |                             | apply/remove coupon
+-------+-------+                             | email cart request
        |                                     +-------+--------+
        v                                             |
+---------------+                                     |
|   EmailAPI    |                                     v
| queue consumer|                            +-------------------+
| send emails   | -------------------------> |    CouponAPI      |
+---------------+        HTTP calls          | coupon lookup/CRUD|
                                              +-------------------+
