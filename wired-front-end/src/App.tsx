import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "./login/login";
import Registro from "./registro/registro";
import ForumList from "./forum/forums";
import CreatePost from "./forum/CriarPost";
import CreateForum from "./forum/forum";

const App: React.FC = () => {
  return (
    <Router>
      <Routes>
        <Route path="/forums" element={<ForumList />} />
        <Route path="/" element={<ForumList />} />
        <Route path="/login" element={<Login />} />
        <Route path="/registro" element={<Registro />} />
        <Route path="/forum/:forumId" element={<CreatePost />} />
        <Route path="/forum/criar" element={<CreateForum />} />
      </Routes>
    </Router>
  );
};

export default App;

