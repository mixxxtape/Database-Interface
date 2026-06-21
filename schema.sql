--
-- PostgreSQL database dump
--

\restrict chzXX83sO98KaN0i0V20Wr8HkjqroMcZPWT8GFLGFqpodPyJWa0IZGplgrsIWYQ

-- Dumped from database version 18.1
-- Dumped by pg_dump version 18.1

-- Started on 2026-06-21 20:09:27

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 225 (class 1259 OID 18334)
-- Name: clothes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.clothes (
    id integer NOT NULL,
    type character varying(20)
);


ALTER TABLE public.clothes OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 18393)
-- Name: customer; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.customer (
    name character varying(20),
    surname character varying(20),
    middle_name character varying(20),
    phone character varying(13) NOT NULL
);


ALTER TABLE public.customer OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 18270)
-- Name: departments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.departments (
    name character varying(20) NOT NULL,
    workers_num integer,
    shop character varying(20) NOT NULL
);


ALTER TABLE public.departments OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 18256)
-- Name: heads; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.heads (
    id integer NOT NULL,
    name character varying(20),
    surname character varying(20),
    middle_name character varying(20),
    shop character varying(20) NOT NULL
);


ALTER TABLE public.heads OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 18356)
-- Name: jewelry; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.jewelry (
    id integer NOT NULL,
    type character varying(20)
);


ALTER TABLE public.jewelry OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 18326)
-- Name: product; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.product (
    id integer NOT NULL,
    brand character varying(20),
    size character varying(10),
    colour character varying(20),
    price numeric,
    name character varying(20),
    owner character varying(20) NOT NULL
);


ALTER TABLE public.product OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 18320)
-- Name: provider; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.provider (
    name character varying(20) NOT NULL,
    address character varying(50)
);


ALTER TABLE public.provider OWNER TO postgres;

--
-- TOC entry 230 (class 1259 OID 18399)
-- Name: purchase; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.purchase (
    product integer NOT NULL,
    customer character varying(13) NOT NULL,
    purchase_date timestamp with time zone NOT NULL,
    total numeric
);


ALTER TABLE public.purchase OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 18345)
-- Name: shoes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shoes (
    id integer NOT NULL,
    season character varying(20)
);


ALTER TABLE public.shoes OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 18250)
-- Name: shops; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.shops (
    name character varying(20) NOT NULL,
    address character varying(50),
    contacts character varying(50)
);


ALTER TABLE public.shops OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 18375)
-- Name: supply; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.supply (
    provider character varying(20) NOT NULL,
    product integer NOT NULL,
    shop character varying(20) NOT NULL,
    amount integer
);


ALTER TABLE public.supply OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 18290)
-- Name: workers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.workers (
    id integer NOT NULL,
    name character varying(20),
    surname character varying(20),
    middle_name character varying(20),
    "position" character varying(20),
    salary numeric,
    department character varying(20) NOT NULL,
    shop character varying(20) NOT NULL,
    manager_id integer
);


ALTER TABLE public.workers OWNER TO postgres;

--
-- TOC entry 5091 (class 0 OID 18334)
-- Dependencies: 225
-- Data for Name: clothes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.clothes (id, type) FROM stdin;
1	Sport
10	Спортивний одяг
11	Класичний одяг
21	Спортивний одяг
90	Повсякденний одяг
41	Повсякденний одяг
99	Верхній одяг
50	Повсякденний одяг
51	Головні убори
\.


--
-- TOC entry 5095 (class 0 OID 18393)
-- Dependencies: 229
-- Data for Name: customer; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.customer (name, surname, middle_name, phone) FROM stdin;
Anna	Koval	Ivanivna	+380991234567
Maria	Bondar	Olehivna	+380931234321
Pavel	Shevchenko	Petrovych	+380671112233
Максим	Чернов	Олександрович	+380671112333
Яна	Мельник	Дмитрівна	+380934445566
Віктор	Павлик	Франкович	+380507778899
Ольга	Тихонова	Юріївна	+380630001122
Дмитро	Всіхбрендів	Валерійович	+380999999999
\.


--
-- TOC entry 5087 (class 0 OID 18270)
-- Dependencies: 221
-- Data for Name: departments; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.departments (name, workers_num, shop) FROM stdin;
Clothes	8	StyleHub
Accessories	3	FashionPoint
Взуття	3	Стиль Центр
Одяг	5	Стиль Центр
Аксесуари	2	Стиль Центр
Взуття	4	Аура Мод
Одяг	4	Аура Мод
Ювелірний	2	Пасаж Гранд
Порожній Деп	0	Пасаж Гранд
Одяг	10	Гранд Маркет
Взуття	8	Гранд Маркет
Ювелірний	1	Гранд Маркет
Shoes	7	UrbanWear
Summer Clothes	10	UrbanWear
\.


--
-- TOC entry 5086 (class 0 OID 18256)
-- Dependencies: 220
-- Data for Name: heads; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.heads (id, name, surname, middle_name, shop) FROM stdin;
1	Ivan	Melnyk	Stepanovych	StyleHub
3	Petro	Kravets	Ivanovych	FashionPoint
1010	Іван	Петренко	Васильович	Стиль Центр
1020	Олена	Коваленко	Ігорівна	Аура Мод
1030	Дмитро	Сидоренко	Миколайович	Пасаж Гранд
1040	Анна	Ткаченко	Олегівна	Аутлет Нова
2	Olena	Tkachenko	Mykolaivna	UrbanWear
\.


--
-- TOC entry 5093 (class 0 OID 18356)
-- Dependencies: 227
-- Data for Name: jewelry; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.jewelry (id, type) FROM stdin;
3	Bracelet
30	Браслети
31	Кулони
43	Каблучки
\.


--
-- TOC entry 5090 (class 0 OID 18326)
-- Dependencies: 224
-- Data for Name: product; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.product (id, brand, size, colour, price, name, owner) FROM stdin;
1	Nike	M	Black	3200	T-shirt	StyleHub
3	Pandora	\N	Silver	2500	Bracelet	FashionPoint
10	Nike	M	Чорний	3200	Худі Sport	Стиль Центр
11	Zara	L	Білий	1800	Сорочка Classic	Стиль Центр
12	Ecco	42	Коричневий	4500	Туфлі Шкіра	Стиль Центр
20	Adidas	41	Білий	3900	Кросівки Run	Аура Мод
21	Puma	S	Червоний	2100	Спортивні штани	Аура Мод
30	Pandora	17	Срібний	2500	Браслет	Пасаж Гранд
31	Swarovski	One Size	Золотий	7800	Підвіска Кристал	Пасаж Гранд
90	H&M	M	Зелений	950	Футболка Basic	Стиль Центр
91	Timberland	43	Жовтий	6200	Черевики Осінь	Аура Мод
40	Crocs	40	Зелений	1500	Сабо Classic	Гранд Маркет
41	Staff	S	Чорний	850	Світшот Urban	Гранд Маркет
42	Casio	One Size	Сірий	3500	Годинник	Гранд Маркет
43	Zarina	16	Срібло	450	Каблучка проста	Гранд Маркет
99	ЛюксБренд	L	Золотий	15000	Куртка Ексклюзив	Гранд Маркет
2	Adidas	42	White	5400	Sneakers	UrbanWear
4	House	M	Grey	2200	Skirt	UrbanWear
50	Tommy Hilfiger	M	Синій	2900	Джинси Slim	Стиль Центр
51	Lacoste	One Size	Білий	1200	Кепка Sport	Аура Мод
\.


--
-- TOC entry 5089 (class 0 OID 18320)
-- Dependencies: 223
-- Data for Name: provider; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.provider (name, address) FROM stdin;
GlobalTextile	Poland
ShoeFactory	Italy
GoldSupplier	Turkey
ІнтерТрейд	м. Одеса, Портова зона, 4
ЄвроМода Поставка	м. Львів, вул. Промислова, 10
ЮвелірГруп	м. Київ, вул. Заводська, 2
Постачальник А	м. Київ, вул. Промислова, 1
Постачальник Б	м. Дніпро, вул. Поля, 2
Постачальник В	м. Одеса, вул. Балківська, 3
Постачальник Г	м. Львів, вул. Зелена, 4
Постачальник Ґ	м. Харків, вул. Полтавський Шлях, 45
\.


--
-- TOC entry 5096 (class 0 OID 18399)
-- Dependencies: 230
-- Data for Name: purchase; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.purchase (product, customer, purchase_date, total) FROM stdin;
2	+380671112233	2025-02-02 15:30:00+02	5400
3	+380931234321	2025-02-03 18:10:00+02	2500
3	+380991234567	2026-05-21 00:00:00+03	4234
10	+380671112233	2026-05-10 14:30:00+03	3200
11	+380671112233	2026-05-12 11:15:00+03	1800
21	+380671112233	2026-05-20 18:00:00+03	2100
20	+380934445566	2026-05-15 12:00:00+03	3900
30	+380507778899	2026-05-25 10:00:00+03	2500
31	+380507778899	2026-05-25 10:05:00+03	7800
10	+380934445566	2026-05-28 16:45:00+03	3200
41	+380671112233	2026-05-31 13:00:00+03	850
1	+380934445566	2026-05-30 00:00:00+03	3213
1	+380931234321	2026-05-31 00:00:00+03	312344
10	+380999999999	2026-06-01 11:00:00+03	3200
11	+380999999999	2026-06-01 11:05:00+03	1800
12	+380999999999	2026-06-01 11:10:00+03	4500
20	+380999999999	2026-06-01 11:15:00+03	3900
21	+380999999999	2026-06-01 11:20:00+03	2100
30	+380999999999	2026-06-01 11:25:00+03	2500
31	+380999999999	2026-06-01 11:30:00+03	7800
40	+380999999999	2026-06-01 11:35:00+03	1500
41	+380999999999	2026-06-01 11:40:00+03	850
42	+380999999999	2026-06-01 11:45:00+03	3500
43	+380999999999	2026-06-01 11:50:00+03	450
50	+380999999999	2026-06-01 11:55:00+03	2900
51	+380999999999	2026-06-01 12:00:00+03	1200
90	+380999999999	2026-06-01 12:05:00+03	950
91	+380999999999	2026-06-01 12:10:00+03	6200
99	+380999999999	2026-06-01 12:15:00+03	15000
4	+380999999999	2026-06-01 13:00:00+03	1200
\.


--
-- TOC entry 5092 (class 0 OID 18345)
-- Dependencies: 226
-- Data for Name: shoes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.shoes (id, season) FROM stdin;
2	Summer
12	Демісезон
20	Літо
91	Осінь-Зима
40	Літо
\.


--
-- TOC entry 5085 (class 0 OID 18250)
-- Dependencies: 219
-- Data for Name: shops; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.shops (name, address, contacts) FROM stdin;
StyleHub	Kyiv, Khreshchatyk 10	+380441112233
FashionPoint	Odesa, Deribasivska 7	+380507778899
Стиль Центр	м. Київ, вул. Хрещатик, 10	+380441112233
Аура Мод	м. Львів, площа Ринок, 5	+380322223344
Пасаж Гранд	м. Одеса, вул. Дерибасівська, 12	+380483334455
Аутлет Нова	м. Харків, вул. Сумська, 25	+380574445566
Порожній Магазин	м. Дніпро, пр. Яворницького, 1	+380560000000
Гранд Маркет	м. Харків, пр. Науки, 12	+380577771122
UrbanWear	Lviv, Svobody Ave 5	+380992223344
\.


--
-- TOC entry 5094 (class 0 OID 18375)
-- Dependencies: 228
-- Data for Name: supply; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.supply (provider, product, shop, amount) FROM stdin;
GlobalTextile	1	StyleHub	50
ShoeFactory	2	FashionPoint	6
ІнтерТрейд	10	Стиль Центр	50
ІнтерТрейд	11	Стиль Центр	100
ЄвроМода Поставка	20	Аура Мод	30
ЄвроМода Поставка	21	Аура Мод	45
ЮвелірГруп	30	Пасаж Гранд	15
ЮвелірГруп	31	Пасаж Гранд	10
Постачальник А	40	Гранд Маркет	20
Постачальник А	41	Гранд Маркет	30
Постачальник Б	40	Гранд Маркет	25
Постачальник Б	41	Гранд Маркет	35
Постачальник В	41	Гранд Маркет	10
Постачальник В	42	Гранд Маркет	15
Постачальник Г	43	Гранд Маркет	100
ShoeFactory	2	UrbanWear	30
Постачальник Ґ	50	Стиль Центр	15
Постачальник Ґ	51	Аура Мод	20
\.


--
-- TOC entry 5088 (class 0 OID 18290)
-- Dependencies: 222
-- Data for Name: workers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.workers (id, name, surname, middle_name, "position", salary, department, shop, manager_id) FROM stdin;
1	Andrii	Boyko	Ivanovych	Consultant	19500	Clothes	StyleHub	\N
3	Dmytro	Teslenko	Olehivych	Cashier	18000	Accessories	FashionPoint	\N
5	пукпукп	упукп	пкупукп	пукпукп	4234	Accessories	FashionPoint	4
5002	Марія	Шевченко	Андріївна	Адміністратор	28000	Взуття	Аура Мод	\N
5003	Андрій	Бойко	Сергійович	Продавець	15000	Одяг	Стиль Центр	5001
5004	Ірина	Лисенко	Михайлівна	Консультант	16000	Одяг	Стиль Центр	5001
5005	Сергій	Клименко	Володимирович	Продавець	15500	Взуття	Аура Мод	5002
5006	Юлія	Кравченко	Віталіївна	Касир	14800	Ювелірний	Пасаж Гранд	\N
6001	Олег	Дмитренко	Петрович	Директор відділу	45000	Одяг	Гранд Маркет	\N
6002	Анна	Рябова	Ігорівна	Консультант	25000	Одяг	Гранд Маркет	6001
6003	Ігор	Семенов	Олегович	Продавець	20000	Одяг	Гранд Маркет	6001
6004	Віталій	Кравс	Андрійович	Старший продавець	18000	Взуття	Гранд Маркет	\N
6005	Світлана	Павлова	Львівна	Касир	12000	Взуття	Гранд Маркет	6004
5001	Олексій	Мороз	Петрович	Manager	25000	Одяг	Стиль Центр	2
31	fwef	312wef	fwef	ffwef	3123	Одяг	Стиль Центр	5001
2	Iryna	Lysenko	Petrivna	Manager	30000	Shoes	UrbanWear	\N
4	Sofia	Ivanenko	Serhiyvna	Consultant	20000	Shoes	UrbanWear	2
\.


--
-- TOC entry 4914 (class 2606 OID 18339)
-- Name: clothes clothes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clothes
    ADD CONSTRAINT clothes_pkey PRIMARY KEY (id);


--
-- TOC entry 4922 (class 2606 OID 18398)
-- Name: customer customer_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customer
    ADD CONSTRAINT customer_pkey PRIMARY KEY (phone);


--
-- TOC entry 4906 (class 2606 OID 18276)
-- Name: departments departments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.departments
    ADD CONSTRAINT departments_pkey PRIMARY KEY (name, shop);


--
-- TOC entry 4902 (class 2606 OID 18262)
-- Name: heads heads_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.heads
    ADD CONSTRAINT heads_pkey PRIMARY KEY (id);


--
-- TOC entry 4904 (class 2606 OID 18264)
-- Name: heads heads_shop_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.heads
    ADD CONSTRAINT heads_shop_key UNIQUE (shop);


--
-- TOC entry 4918 (class 2606 OID 18361)
-- Name: jewelry jewelry_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.jewelry
    ADD CONSTRAINT jewelry_pkey PRIMARY KEY (id);


--
-- TOC entry 4912 (class 2606 OID 18333)
-- Name: product product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_pkey PRIMARY KEY (id);


--
-- TOC entry 4910 (class 2606 OID 18325)
-- Name: provider provider_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.provider
    ADD CONSTRAINT provider_pkey PRIMARY KEY (name);


--
-- TOC entry 4924 (class 2606 OID 18424)
-- Name: purchase purchase_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.purchase
    ADD CONSTRAINT purchase_pkey PRIMARY KEY (product, customer, purchase_date);


--
-- TOC entry 4916 (class 2606 OID 18350)
-- Name: shoes shoes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shoes
    ADD CONSTRAINT shoes_pkey PRIMARY KEY (id);


--
-- TOC entry 4900 (class 2606 OID 18255)
-- Name: shops shops_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shops
    ADD CONSTRAINT shops_pkey PRIMARY KEY (name);


--
-- TOC entry 4920 (class 2606 OID 18429)
-- Name: supply supply_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supply
    ADD CONSTRAINT supply_pkey PRIMARY KEY (provider, product, shop);


--
-- TOC entry 4908 (class 2606 OID 18297)
-- Name: workers workers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.workers
    ADD CONSTRAINT workers_pkey PRIMARY KEY (id);


--
-- TOC entry 4930 (class 2606 OID 35519)
-- Name: clothes clothes_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clothes
    ADD CONSTRAINT clothes_id_fkey FOREIGN KEY (id) REFERENCES public.product(id) DEFERRABLE;


--
-- TOC entry 4926 (class 2606 OID 35504)
-- Name: departments departments_shop_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.departments
    ADD CONSTRAINT departments_shop_fkey FOREIGN KEY (shop) REFERENCES public.shops(name) DEFERRABLE;


--
-- TOC entry 4925 (class 2606 OID 35499)
-- Name: heads heads_shop_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.heads
    ADD CONSTRAINT heads_shop_fkey FOREIGN KEY (shop) REFERENCES public.shops(name) DEFERRABLE;


--
-- TOC entry 4932 (class 2606 OID 35529)
-- Name: jewelry jewelry_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.jewelry
    ADD CONSTRAINT jewelry_id_fkey FOREIGN KEY (id) REFERENCES public.product(id) DEFERRABLE;


--
-- TOC entry 4929 (class 2606 OID 35494)
-- Name: product product_owner_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.product
    ADD CONSTRAINT product_owner_fkey FOREIGN KEY (owner) REFERENCES public.shops(name) DEFERRABLE;


--
-- TOC entry 4936 (class 2606 OID 35539)
-- Name: purchase purchase_customer_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.purchase
    ADD CONSTRAINT purchase_customer_fkey FOREIGN KEY (customer) REFERENCES public.customer(phone) DEFERRABLE;


--
-- TOC entry 4937 (class 2606 OID 35534)
-- Name: purchase purchase_product_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.purchase
    ADD CONSTRAINT purchase_product_fkey FOREIGN KEY (product) REFERENCES public.product(id) DEFERRABLE;


--
-- TOC entry 4931 (class 2606 OID 35524)
-- Name: shoes shoes_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.shoes
    ADD CONSTRAINT shoes_id_fkey FOREIGN KEY (id) REFERENCES public.product(id) DEFERRABLE;


--
-- TOC entry 4933 (class 2606 OID 35484)
-- Name: supply supply_product_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supply
    ADD CONSTRAINT supply_product_fkey FOREIGN KEY (product) REFERENCES public.product(id) DEFERRABLE;


--
-- TOC entry 4934 (class 2606 OID 35489)
-- Name: supply supply_provider_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supply
    ADD CONSTRAINT supply_provider_fkey FOREIGN KEY (provider) REFERENCES public.provider(name) DEFERRABLE;


--
-- TOC entry 4935 (class 2606 OID 35479)
-- Name: supply supply_shop_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.supply
    ADD CONSTRAINT supply_shop_fkey FOREIGN KEY (shop) REFERENCES public.shops(name) DEFERRABLE;


--
-- TOC entry 4927 (class 2606 OID 35509)
-- Name: workers workers_department_shop_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.workers
    ADD CONSTRAINT workers_department_shop_fkey FOREIGN KEY (department, shop) REFERENCES public.departments(name, shop) DEFERRABLE;


--
-- TOC entry 4928 (class 2606 OID 35514)
-- Name: workers workers_manager_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.workers
    ADD CONSTRAINT workers_manager_fkey FOREIGN KEY (manager_id) REFERENCES public.workers(id) ON DELETE SET NULL DEFERRABLE;


-- Completed on 2026-06-21 20:09:27

--
-- PostgreSQL database dump complete
--

\unrestrict chzXX83sO98KaN0i0V20Wr8HkjqroMcZPWT8GFLGFqpodPyJWa0IZGplgrsIWYQ

