import React, { useEffect, useState } from "react";
import { getProducts } from "../Services/api";
import { Card, Button, Container, Row, Col } from "react-bootstrap";
import { useDispatch, useSelector } from "react-redux";
import { addToCart } from "../redux/cartSlice";
import defaultImage from "../assets/product/photo.png";
import { toast } from "react-toastify";

const API_URL = "https://localhost:7048";

const Home = () => {
  const [products, setProducts] = useState([]);
  const dispatch = useDispatch();
  const cartItems = useSelector((state) => state.cart);

  useEffect(() => {
    getProducts().then((res) => setProducts(res.data));
  }, []);

  const handleAddToCart = (product) => {
    const exists = cartItems.find((item) => item.id === product.productId);

    if (exists) {
      toast.info(`${product.name} quantity increased in cart`);
    } else {
      toast.success(`${product.name} added to cart`);
    }

    dispatch(addToCart(product));
  };

  const getProductImage = (imageUrl) => {
    return imageUrl ? `${API_URL}/${imageUrl}` : defaultImage;
  };

  return (
    <Container className="mt-4">
      <Row>
        {products.map((product) => (
          <Col md={4} key={product.productId}>
            <Card className="mb-4 shadow-sm">
              <Card.Img
                variant="top"
                src={getProductImage(product.imageUrl)}
                style={{ height: "200px", objectFit: "cover" }}
                alt={product.name}
              />
              <Card.Body>
                <Card.Title>{product.name}</Card.Title>
                <Card.Text>${product.price}</Card.Text>
                <Button
                  variant="primary"
                  onClick={() => handleAddToCart(product)}
                >
                  Add to Cart
                </Button>
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    </Container>
  );
};

export default Home;
