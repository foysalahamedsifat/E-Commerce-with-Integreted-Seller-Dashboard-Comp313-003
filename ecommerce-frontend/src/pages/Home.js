import React, { useEffect, useState } from "react";
import { getProducts } from "../Services/api";
import { Card, Button, Container, Row, Col } from "react-bootstrap";
import { useDispatch } from "react-redux";
import { addToCart } from "../redux/cartSlice";

const Home = () => {
  const [products, setProducts] = useState([]);
  const dispatch = useDispatch();

  useEffect(() => {
    getProducts().then((res) => setProducts(res.data));
  }, []);

  return (
    <Container className="mt-4">
      <Row>
        {products.map((product) => (
          <Col md={4} key={product.id}>
            <Card className="mb-4">
              <Card.Img variant="top" src={product.image} />
              <Card.Body>
                <Card.Title>{product.name}</Card.Title>
                <Card.Text>${product.price}</Card.Text>
                <Button variant="primary" onClick={() => dispatch(addToCart(product))}>
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
