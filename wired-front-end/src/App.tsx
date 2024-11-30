import React from "react";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import CreatePost from "./forum/CriarPost";
import CreateForum from "./forum/forum";
import ForumList from "./forum/forums";
import Login from "./login/login";
import Registro from "./registro/registro";

const App: React.FC = () => {
  return (
    <Router>
      <Routes>
        <Route path="/forums" element={<ForumList />} />
        <Route path="/" element={<Login />} />
        <Route path="/registro" element={<Registro />} />
        <Route path="/forums" element={<ForumList />} />
        <Route path="/forum/:forumId" element={<CreatePost />} />
        <Route path="/forum/criar" element={<CreateForum />} />
      </Routes>
    </Router>
  );
};

export default App;

