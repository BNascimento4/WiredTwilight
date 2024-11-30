import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import ForumList from './forums';
import CreatePost from './CriarPost';

const App: React.FC = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<ForumList />} />
                <Route path="/forum/:forumId" element={<CreatePost />} />
            </Routes>
        </Router>
    );
};

export default App;
